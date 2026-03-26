import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { ApiService } from '../../../core/services/api.service';

export interface ApplicationLog {
  id: number;
  message: string;
  logLevel: string;
  timestamp: string;
  eventId: number;
  eventDescription: string;
  category: string;
  exception?: string | null;
}

export interface EventIdDto {
  id: number;
  name?: string;
}

export interface ApplicationLogQuery {
  logLevel?: string;
  categories?: string[];
  events?: number[];
}

export type SystemStatus = 'Ok' | 'Maintenance';

@Injectable({
  providedIn: 'root'
})
export class SystemService {
  private apiUrl = `${environment.apiUrl}/system`;
  private apiService = inject(ApiService);
  private http = inject(HttpClient);

  ping(): Observable<unknown> {
    return this.apiService.head<unknown>(`${this.apiUrl}/ping`);
  }

  getDatabaseLogs(query: ApplicationLogQuery): Observable<ApplicationLog[]> {
    let params = new HttpParams();

    if (query.logLevel) {
      params = params.set('LogLevel', query.logLevel);
    }

    (query.categories ?? []).forEach(category => {
      params = params.append('Categories', category);
    });

    (query.events ?? []).forEach(eventId => {
      params = params.append('Events', eventId.toString());
    });

    return this.apiService.get<ApplicationLog[]>(`${this.apiUrl}/logs/db`, params);
  }

  getDatabaseLogCategories(): Observable<string[]> {
    return this.apiService.get<string[]>(`${this.apiUrl}/logs/db/categories`);
  }

  getDatabaseLogEvents(): Observable<EventIdDto[]> {
    return this.apiService.get<EventIdDto[]>(`${this.apiUrl}/logs/db/events`);
  }

  getLogFileNames(): Observable<string[]> {
    return this.apiService.get<string[]>(`${this.apiUrl}/logs/file/list`);
  }

  downloadLogFile(fileName: string): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/logs/file/${encodeURIComponent(fileName)}`, {
      responseType: 'blob'
    });
  }

  getSettings(): Observable<unknown> {
    return this.apiService.get<unknown>(`${this.apiUrl}/settings`);
  }

  getSettingsSchema(): Observable<unknown> {
    return this.apiService.get<unknown>(`${this.apiUrl}/settings/schema`);
  }

  getDefaultSettings(): Observable<unknown> {
    return this.apiService.get<unknown>(`${this.apiUrl}/settings/default`);
  }

  updateSettings(payload: unknown): Observable<unknown> {
    return this.apiService.put<unknown>(`${this.apiUrl}/settings/update`, payload);
  }

  patchSettings(payload: unknown): Observable<unknown> {
    return this.apiService.patch<unknown>(`${this.apiUrl}/settings/patch`, payload);
  }

  exportSettings(): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/settings/export`, {
      responseType: 'blob'
    });
  }

  importSettings(file: File): Observable<unknown> {
    const formData = new FormData();
    formData.append('file', file, file.name);

    return this.http.put(`${this.apiUrl}/settings/import`, formData);
  }

  getSystemStatus(): Observable<SystemStatus | number | string> {
    return this.apiService.get<SystemStatus | number | string>(`${this.apiUrl}/status`);
  }

  setMaintenanceMode(enabled: boolean): Observable<unknown> {
    const params = new HttpParams().set('enabled', String(enabled));
    return this.apiService.post<unknown>(`${this.apiUrl}/maintenance`, {}, params);
  }
}

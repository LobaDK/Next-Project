import { HttpErrorResponse } from '@angular/common/http';
import { Injectable, Signal, signal } from '@angular/core';

@Injectable({
    providedIn: 'root'
})
export class MaintenanceService {
    private readonly _isMaintenanceEnabled = signal(false);

    readonly isMaintenanceEnabled: Signal<boolean> = this._isMaintenanceEnabled.asReadonly();

    setMaintenanceEnabled(enabled: boolean): void {
        this._isMaintenanceEnabled.set(enabled);
    }

    markMaintenanceFromError(error: HttpErrorResponse): void {
        if (this.isMaintenanceResponse(error)) {
            this._isMaintenanceEnabled.set(true);
        }
    }

    private isMaintenanceResponse(error: HttpErrorResponse): boolean {
        return error.status === 503;
    }
}
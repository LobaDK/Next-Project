import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { IHomeService } from '../../../core/interfaces/service.interfaces';

@Injectable({
  providedIn: 'root'
})
export class MockHomeService implements IHomeService {
  // Mock method to simulate active questionnaire check
  checkForExistingActiveQuestionnaires(): Observable<{ exists: boolean; id: string | null }> {
    // Simulate checking for existing questionnaires for mock user
    return of({ exists: true, id: 'active1' });
  }
}

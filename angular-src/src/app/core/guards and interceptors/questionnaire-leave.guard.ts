import { CanDeactivateFn } from '@angular/router';
import { Observable } from 'rxjs';
import type { QuestionnaireComponent } from '../../features/questionnaire/questionnaire.component';

export const questionnaireLeaveGuard: CanDeactivateFn<QuestionnaireComponent> = (
    component
): boolean | Observable<boolean> => {
    return component.canDeactivate();
};

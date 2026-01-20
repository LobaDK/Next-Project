import { Routes } from '@angular/router';
import { PageNotFoundComponent } from './core/components/page-not-found/page-not-found.component';
import { authGuard } from './core/guards and interceptors/auth.guard';
import { roleGuard } from './core/guards and interceptors/role-guard.guard';
import { Role } from './shared/models/user.model';
import { TestComponent } from './features/test/test.component';
import { TableExampleComponent } from './features/table-example/table-example.component';
import { QuestionnaireResultsComponent } from './features/questionnaire-results/questionnaire-results.component';
import { LoginComponent } from './features/login/login.component';

export const routes: Routes = [
  {path: 'login', component: LoginComponent},
  { path: '', redirectTo: '/test', pathMatch: 'full' },
  { path: 'test', component: TestComponent },
  { path: 'table-example', component: TableExampleComponent },
  { path: 'questionnaire-results', component: QuestionnaireResultsComponent },
  { path: '**', component: PageNotFoundComponent }
];
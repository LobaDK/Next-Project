import { Routes } from '@angular/router';
import { PageNotFoundComponent } from './core/components/page-not-found/page-not-found.component';
import { authGuard } from './core/guards and interceptors/auth.guard';
import { roleGuard } from './core/guards and interceptors/role-guard.guard';
import { Role } from './shared/models/user.model';
import { TestComponent } from './features/test/test.component';
import { QuestionnaireResultsComponent } from './features/questionnaire-results/questionnaire-results.component';
import { LoginComponent } from './features/login/login.component';
import { HomeComponent } from './features/home/home.component';
import { TemplateEditComponent } from './features/Template/template-edit/template-edit.component';
import { TemplateListComponent } from './features/Template/template-list/template-list.component';

export const routes: Routes = [
  {path: 'login', component: LoginComponent},
  {path: 'home', component: HomeComponent, canActivate: [authGuard]},
  { path: 'template-list', component: TemplateListComponent, canActivate: [authGuard] },
  { path: 'template-edit', component: TemplateEditComponent, canActivate: [authGuard] },
  { path: '', redirectTo: '/test', pathMatch: 'full' },
  { path: 'test', component: TestComponent, canActivate: [authGuard] },
  { path: 'questionnaire-results', component: QuestionnaireResultsComponent, canActivate: [authGuard] },
  { path: '**', component: PageNotFoundComponent }
];
import { ApplicationConfig, importProvidersFrom, inject, provideAppInitializer, Type } from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import {  provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { jwtInterceptor } from './core/guards and interceptors/jwt.interceptor';
import { maintenanceInterceptor } from './core/guards and interceptors/maintenance.interceptor';
import { AuthService } from './core/services/auth.service';
import { environment } from '../environments/environment';
import { MockAuthService } from './core/services/mock/mock.auth.service';
import { HomeService } from './features/home/services/home.service';
import { MockHomeService } from './features/home/services/mock.home.service';
import { AnswerService } from './features/questionnaire/services/answer.service';
import { MockAnswerService } from './features/questionnaire/services/mock.answer.service';
import { TemplateService } from './features/template-manager/services/template.service';
import { MockTemplateService } from './features/template-manager/services/mock-template.service';
import { ActiveService } from './features/active-questionnaire-manager/services/active.service';
import { MockActiveService } from './features/active-questionnaire-manager/services/mock.active.service';
import { ResultService } from './features/result/services/result.service';
import { MockResultService } from './features/result/services/mock.result.service';
import { TeacherService } from './features/teacher-dashboard/services/teacher.service';
import { MockTeacherService } from './features/teacher-dashboard/services/mock.teacher.service';
import { provideTranslateService } from '@ngx-translate/core';
import { provideTranslateHttpLoader } from '@ngx-translate/http-loader';
import { I18nService } from './core/services/I18n.service';
import { IAuthService, IHomeService, IAnswerService, ITemplateService, IActiveService, IResultService, ITeacherService } from './core/interfaces/service.interfaces';




export const appConfig: ApplicationConfig = {
  providers: [provideRouter(routes),
    provideHttpClient(
    withInterceptors([jwtInterceptor, maintenanceInterceptor]),
  ), provideAnimationsAsync(),
    // Type-safe mock service providers
    mockService<IAuthService>(AuthService, MockAuthService),
    mockService<IHomeService>(HomeService, MockHomeService),
    mockService<IAnswerService>(AnswerService, MockAnswerService),
    mockService<ITemplateService>(TemplateService, MockTemplateService),
    mockService<IActiveService>(ActiveService, MockActiveService),
    mockService<IResultService>(ResultService, MockResultService),
    mockService<ITeacherService>(TeacherService, MockTeacherService),
    provideAppInitializer(() => {
      const authService = inject(AuthService);
      return authService.initializeAuthState();
    }),
    provideTranslateService({
      lang: I18nService.getInitialLanguage(),            
      fallbackLang: 'da',
      loader: provideTranslateHttpLoader({
        prefix: '/assets/i18n/',  
        suffix: '.json',          
      }),
    }),
  ],
};

/**
 * Creates a provider that switches between real and mock service implementations
 * based on environment.useMock setting.
 * @param real - The real service implementation
 * @param mock - The mock service implementation  
 */
function mockService<TInterface>(real: Type<TInterface>, mock: Type<TInterface>) {
  return {
    provide: real,
    useClass: environment.useMock ? mock : real
  };
}

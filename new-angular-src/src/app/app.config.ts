import { ApplicationConfig, importProvidersFrom, inject, provideAppInitializer, Type } from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import {  provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { jwtInterceptor } from './core/guards and interceptors/jwt.interceptor';
import { AuthService } from './core/services/auth.service';
import { environment } from '../environments/environment';
import { provideTranslateService } from '@ngx-translate/core';
import { provideTranslateHttpLoader } from '@ngx-translate/http-loader';
import { I18nService } from './core/services/I18n.service';




export const appConfig: ApplicationConfig = {
  providers: [provideRouter(routes),
    provideHttpClient(
    withInterceptors([jwtInterceptor]),
  ), provideAnimationsAsync(),
    // Type-safe mock service providers
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

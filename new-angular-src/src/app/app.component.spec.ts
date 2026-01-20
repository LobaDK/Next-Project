import { TestBed } from '@angular/core/testing';
import { AppComponent } from './app.component';


import { APP_INITIALIZER, ApplicationConfig, importProvidersFrom } from '@angular/core';
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

describe('AppComponent (standalone)', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AppComponent],
      providers: [
        // router
        provideRouter(routes),

        // http client + interceptors
        provideHttpClient(withInterceptors([jwtInterceptor])),

        // animations
        provideAnimationsAsync(),
        // i18n / translate
        provideTranslateService({
          lang: I18nService.getInitialLanguage(),
          fallbackLang: 'da',
          loader: provideTranslateHttpLoader({
            prefix: '/assets/i18n/',
            suffix: '.json',
          }),
        }),
      ],
    }).compileComponents();
  });

  it('should create the app', () => {
    const fixture = TestBed.createComponent(AppComponent);
    const app = fixture.componentInstance;
    expect(app).toBeTruthy();
  });
});

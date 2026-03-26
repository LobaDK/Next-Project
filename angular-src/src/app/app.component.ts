
import { Component, computed, effect, inject, signal } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { RouterModule, RouterOutlet } from '@angular/router';
import { filter } from 'rxjs';

import {HeaderComponent } from './core/components/app-header/header.component';
import { TranslateModule } from '@ngx-translate/core';
import { AuthService } from './core/services/auth.service';
import { MaintenanceService } from './core/services/maintenance.service';

@Component({
    selector: 'app-root',
        imports: [RouterModule, RouterOutlet, HeaderComponent, TranslateModule],
    templateUrl: 'app.component.html',
    styleUrls: ['app.component.css']
})
export class AppComponent {
    private router = inject(Router);
    private authService = inject(AuthService);
    private maintenanceService = inject(MaintenanceService);
    readonly isAuthenticated = this.authService.isAuthenticated;
    readonly isMaintenanceEnabled = this.maintenanceService.isMaintenanceEnabled;
    private readonly isMaintenanceDismissed = signal(false);
    private readonly currentUrl = signal(this.router.url);
    readonly isLoginPage = computed(() => this.currentUrl() === '/' && !this.isAuthenticated());
    readonly showMaintenanceBanner = computed(
        () => this.isMaintenanceEnabled() && !this.isMaintenanceDismissed() && !this.isLoginPage()
    );

    constructor() {
        this.router.events
            .pipe(filter((event): event is NavigationEnd => event instanceof NavigationEnd))
            .subscribe(event => this.currentUrl.set(event.urlAfterRedirects));

        effect(() => {
            if (!this.isMaintenanceEnabled()) {
                this.isMaintenanceDismissed.set(false);
            }
        });
    }

    dismissMaintenanceBanner(): void {
        this.isMaintenanceDismissed.set(true);
    }
}

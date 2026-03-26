import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { MaintenanceService } from '../services/maintenance.service';

export const maintenanceInterceptor: HttpInterceptorFn = (req, next) => {
    const maintenanceService = inject(MaintenanceService);

    return next(req).pipe(
        catchError((error: HttpErrorResponse) => {
            maintenanceService.markMaintenanceFromError(error);
            return throwError(() => error);
        })
    );
};
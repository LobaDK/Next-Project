import { Component, inject } from '@angular/core';
import { AuthService } from '../../core/services/auth.service';
import { TranslateModule } from '@ngx-translate/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-home',
  imports: [TranslateModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {
  private authService = inject(AuthService);
  private router = inject(Router);

  user = this.authService.user;

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}

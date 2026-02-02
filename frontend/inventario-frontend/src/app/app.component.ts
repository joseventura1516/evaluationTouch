import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet, RouterLink, RouterLinkActive, Router } from '@angular/router';
import { AuthService } from './services/auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive],
  template: `
    <nav class="navbar" *ngIf="authService.isLoggedIn()">
      <a routerLink="/productos" class="navbar-brand">Sistema de Inventario</a>
      <ul class="navbar-nav">
        <li><a routerLink="/productos" routerLinkActive="active">Productos</a></li>
        <li *ngIf="authService.isAdmin()"><a routerLink="/reportes" routerLinkActive="active">Reportes</a></li>
        <li *ngIf="authService.isAdmin()"><a routerLink="/notificaciones" routerLinkActive="active">
          Notificaciones
          <span class="badge badge-danger" *ngIf="unreadCount > 0">{{ unreadCount }}</span>
        </a></li>
        <li><a href="#" (click)="logout($event)">Cerrar Sesión</a></li>
      </ul>
    </nav>
    <main [class.container]="authService.isLoggedIn()">
      <router-outlet></router-outlet>
    </main>
  `,
  styles: [`
    .badge {
      margin-left: 5px;
      font-size: 10px;
      padding: 2px 6px;
    }
  `]
})
export class AppComponent {
  unreadCount = 0;

  constructor(
    public authService: AuthService,
    private router: Router
  ) {
    if (this.authService.isLoggedIn() && this.authService.isAdmin()) {
      this.loadUnreadCount();
    }
  }

  loadUnreadCount(): void {
    // This would be loaded from notification service
  }

  logout(event: Event): void {
    event.preventDefault();
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}

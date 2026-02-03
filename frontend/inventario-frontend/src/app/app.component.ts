import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet, RouterLink, RouterLinkActive, Router, NavigationEnd } from '@angular/router';
import { AuthService } from './services/auth.service';
import { NotificacionesService } from './services/notificaciones.service';
import { Subscription, interval, filter } from 'rxjs';

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
        <li *ngIf="authService.isAdmin()">
          <a routerLink="/notificaciones" routerLinkActive="active" class="notification-link">
            Notificaciones
            <span class="notification-badge" *ngIf="unreadCount > 0">{{ unreadCount > 99 ? '99+' : unreadCount }}</span>
          </a>
        </li>
        <li><a href="#" (click)="logout($event)">Cerrar Sesión</a></li>
      </ul>
    </nav>
    <main [class.container]="authService.isLoggedIn()">
      <router-outlet></router-outlet>
    </main>
  `,
  styles: [`
    .notification-link {
      position: relative;
    }
    .notification-badge {
      position: absolute;
      top: -8px;
      right: -12px;
      background-color: #e74c3c;
      color: white;
      font-size: 11px;
      font-weight: bold;
      padding: 2px 6px;
      border-radius: 10px;
      min-width: 18px;
      text-align: center;
      animation: pulse 2s infinite;
    }
    @keyframes pulse {
      0% { transform: scale(1); }
      50% { transform: scale(1.1); }
      100% { transform: scale(1); }
    }
  `]
})
export class AppComponent implements OnInit, OnDestroy {
  unreadCount = 0;
  private refreshInterval?: Subscription;
  private routerSubscription?: Subscription;

  constructor(
    public authService: AuthService,
    private router: Router,
    private notificacionesService: NotificacionesService
  ) {}

  ngOnInit(): void {
    if (this.authService.isLoggedIn() && this.authService.isAdmin()) {
      this.loadUnreadCount();
      this.startAutoRefresh();
    }

    // Refresh count when navigating
    this.routerSubscription = this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe(() => {
        if (this.authService.isLoggedIn() && this.authService.isAdmin()) {
          this.loadUnreadCount();
        }
      });
  }

  ngOnDestroy(): void {
    this.refreshInterval?.unsubscribe();
    this.routerSubscription?.unsubscribe();
  }

  loadUnreadCount(): void {
    this.notificacionesService.getUnreadCount().subscribe({
      next: (response) => {
        if (response.success) {
          this.unreadCount = response.count;
        }
      },
      error: () => {
        // Silently fail - don't disrupt user experience
      }
    });
  }

  private startAutoRefresh(): void {
    // Refresh every 30 seconds
    this.refreshInterval = interval(30000).subscribe(() => {
      if (this.authService.isLoggedIn() && this.authService.isAdmin()) {
        this.loadUnreadCount();
      }
    });
  }

  logout(event: Event): void {
    event.preventDefault();
    this.refreshInterval?.unsubscribe();
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}

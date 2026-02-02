import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NotificacionesService, Notification } from '../services/notificaciones.service';

@Component({
  selector: 'app-notificaciones',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="notificaciones-container">
      <div class="header">
        <h2>Notificaciones</h2>
        <button
          *ngIf="notifications.length > 0"
          class="btn btn-secondary"
          (click)="markAllAsRead()"
          [disabled]="loading"
        >
          Marcar todas como leídas
        </button>
      </div>

      <div class="alert alert-danger" *ngIf="error">{{ error }}</div>

      <div *ngIf="loading" class="text-center">
        <div class="spinner"></div>
        <p>Cargando notificaciones...</p>
      </div>

      <div *ngIf="!loading && notifications.length === 0" class="alert alert-info">
        No tienes notificaciones.
      </div>

      <div class="notifications-list" *ngIf="!loading && notifications.length > 0">
        <div
          *ngFor="let notification of notifications"
          class="notification-card card"
          [class.unread]="!notification.isRead"
        >
          <div class="notification-content">
            <div class="notification-icon" [ngClass]="getIconClass(notification.type)">
              {{ getIcon(notification.type) }}
            </div>
            <div class="notification-body">
              <p class="notification-message">{{ notification.message }}</p>
              <span class="notification-date">{{ notification.createdAt | date:'dd/MM/yyyy HH:mm' }}</span>
            </div>
          </div>
          <button
            *ngIf="!notification.isRead"
            class="btn btn-primary btn-sm"
            (click)="markAsRead(notification)"
          >
            Marcar como leída
          </button>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .notificaciones-container {
      padding: 20px 0;
    }

    .header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 20px;
    }

    .header h2 {
      margin: 0;
      color: #2c3e50;
    }

    .notifications-list {
      display: flex;
      flex-direction: column;
      gap: 15px;
    }

    .notification-card {
      display: flex;
      justify-content: space-between;
      align-items: center;
      transition: background-color 0.3s;
    }

    .notification-card.unread {
      background-color: #e8f4fd;
      border-left: 4px solid #3498db;
    }

    .notification-content {
      display: flex;
      align-items: flex-start;
      gap: 15px;
    }

    .notification-icon {
      width: 40px;
      height: 40px;
      border-radius: 50%;
      display: flex;
      align-items: center;
      justify-content: center;
      font-size: 18px;
    }

    .notification-icon.warning {
      background-color: #f39c12;
      color: white;
    }

    .notification-icon.info {
      background-color: #3498db;
      color: white;
    }

    .notification-icon.error {
      background-color: #e74c3c;
      color: white;
    }

    .notification-body {
      flex: 1;
    }

    .notification-message {
      margin: 0 0 5px 0;
      color: #2c3e50;
    }

    .notification-date {
      font-size: 12px;
      color: #95a5a6;
    }

    .text-center {
      text-align: center;
      padding: 40px;
    }

    .btn-sm {
      padding: 5px 15px;
      font-size: 12px;
    }
  `]
})
export class NotificacionesComponent implements OnInit {
  notifications: Notification[] = [];
  loading = false;
  error = '';

  constructor(private notificacionesService: NotificacionesService) {}

  ngOnInit(): void {
    this.loadNotifications();
  }

  loadNotifications(): void {
    this.loading = true;
    this.error = '';

    this.notificacionesService.getNotifications().subscribe({
      next: (response) => {
        this.loading = false;
        if (response.success) {
          this.notifications = response.data;
        } else {
          this.error = response.message;
        }
      },
      error: (err) => {
        this.loading = false;
        this.error = err.error?.message || 'Error al cargar notificaciones';
      }
    });
  }

  markAsRead(notification: Notification): void {
    this.notificacionesService.markAsRead(notification.id).subscribe({
      next: (response) => {
        if (response.success) {
          notification.isRead = true;
        }
      },
      error: (err) => {
        this.error = err.error?.message || 'Error al marcar notificación';
      }
    });
  }

  markAllAsRead(): void {
    this.notificacionesService.markAllAsRead().subscribe({
      next: (response) => {
        if (response.success) {
          this.notifications.forEach(n => n.isRead = true);
        }
      },
      error: (err) => {
        this.error = err.error?.message || 'Error al marcar notificaciones';
      }
    });
  }

  getIcon(type: string): string {
    switch (type.toLowerCase()) {
      case 'warning': return '!';
      case 'error': return 'X';
      default: return 'i';
    }
  }

  getIconClass(type: string): string {
    return type.toLowerCase();
  }
}

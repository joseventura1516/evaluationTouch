import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface Notification {
  id: number;
  message: string;
  type: string;
  isRead: boolean;
  createdAt: string;
}

export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T;
}

@Injectable({ providedIn: 'root' })
export class NotificacionesService {
  private apiUrl = `${environment.apiUrl}/notifications`;

  constructor(private http: HttpClient) {}

  getNotifications(): Observable<ApiResponse<Notification[]>> {
    return this.http.get<ApiResponse<Notification[]>>(this.apiUrl);
  }

  getUnreadNotifications(): Observable<ApiResponse<Notification[]>> {
    return this.http.get<ApiResponse<Notification[]>>(`${this.apiUrl}/unread`);
  }

  getUnreadCount(): Observable<{ success: boolean; count: number }> {
    return this.http.get<{ success: boolean; count: number }>(`${this.apiUrl}/unread-count`);
  }

  markAsRead(id: number): Observable<ApiResponse<Notification>> {
    return this.http.put<ApiResponse<Notification>>(`${this.apiUrl}/${id}/mark-read`, {});
  }

  markAllAsRead(): Observable<ApiResponse<boolean>> {
    return this.http.put<ApiResponse<boolean>>(`${this.apiUrl}/mark-all-read`, {});
  }
}

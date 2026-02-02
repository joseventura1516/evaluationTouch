import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ReportesService {
  private apiUrl = `${environment.apiUrl}/reports`;

  constructor(private http: HttpClient) {}

  downloadLowStockReport(): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/low-stock-pdf`, {
      responseType: 'blob'
    });
  }

  downloadInventoryReport(): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/inventory-pdf`, {
      responseType: 'blob'
    });
  }
}

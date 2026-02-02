import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReportesService } from '../services/reportes.service';

@Component({
  selector: 'app-reportes',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="reportes-container">
      <h2>Generación de Reportes</h2>

      <div class="alert alert-danger" *ngIf="error">{{ error }}</div>
      <div class="alert alert-success" *ngIf="success">{{ success }}</div>

      <div class="reports-grid">
        <div class="card report-card">
          <div class="card-header">
            <h3 class="card-title">Reporte de Inventario Bajo</h3>
          </div>
          <p>Genera un reporte PDF con todos los productos que tienen menos de 5 unidades en stock.</p>
          <button
            class="btn btn-primary"
            (click)="downloadLowStockReport()"
            [disabled]="loadingLowStock"
          >
            <span *ngIf="loadingLowStock">Generando...</span>
            <span *ngIf="!loadingLowStock">Descargar PDF</span>
          </button>
        </div>

        <div class="card report-card">
          <div class="card-header">
            <h3 class="card-title">Reporte General de Inventario</h3>
          </div>
          <p>Genera un reporte PDF completo con todos los productos del inventario.</p>
          <button
            class="btn btn-success"
            (click)="downloadInventoryReport()"
            [disabled]="loadingInventory"
          >
            <span *ngIf="loadingInventory">Generando...</span>
            <span *ngIf="!loadingInventory">Descargar PDF</span>
          </button>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .reportes-container {
      padding: 20px 0;
    }

    h2 {
      color: #2c3e50;
      margin-bottom: 30px;
    }

    .reports-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
      gap: 20px;
    }

    .report-card {
      text-align: center;
    }

    .report-card p {
      color: #7f8c8d;
      margin-bottom: 20px;
    }

    .report-card button {
      width: 100%;
    }
  `]
})
export class ReportesComponent {
  loadingLowStock = false;
  loadingInventory = false;
  error = '';
  success = '';

  constructor(private reportesService: ReportesService) {}

  downloadLowStockReport(): void {
    this.loadingLowStock = true;
    this.error = '';
    this.success = '';

    this.reportesService.downloadLowStockReport().subscribe({
      next: (blob) => {
        this.loadingLowStock = false;
        this.downloadFile(blob, `Reporte_Inventario_Bajo_${this.getDateString()}.pdf`);
        this.success = 'Reporte descargado exitosamente';
      },
      error: (err) => {
        this.loadingLowStock = false;
        this.error = 'Error al generar el reporte';
        console.error(err);
      }
    });
  }

  downloadInventoryReport(): void {
    this.loadingInventory = true;
    this.error = '';
    this.success = '';

    this.reportesService.downloadInventoryReport().subscribe({
      next: (blob) => {
        this.loadingInventory = false;
        this.downloadFile(blob, `Reporte_Inventario_General_${this.getDateString()}.pdf`);
        this.success = 'Reporte descargado exitosamente';
      },
      error: (err) => {
        this.loadingInventory = false;
        this.error = 'Error al generar el reporte';
        console.error(err);
      }
    });
  }

  private downloadFile(blob: Blob, filename: string): void {
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = filename;
    link.click();
    window.URL.revokeObjectURL(url);
  }

  private getDateString(): string {
    const date = new Date();
    return `${date.getFullYear()}${(date.getMonth() + 1).toString().padStart(2, '0')}${date.getDate().toString().padStart(2, '0')}`;
  }
}

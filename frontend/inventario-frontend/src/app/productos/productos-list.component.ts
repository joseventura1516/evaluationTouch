import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ProductosService, Product } from '../services/productos.service';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-productos-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  template: `
    <div class="productos-container">
      <div class="header">
        <h2>Gestión de Productos</h2>
        <a *ngIf="authService.isAdmin()" routerLink="/productos/nuevo" class="btn btn-success">
          + Nuevo Producto
        </a>
      </div>

      <div class="filters card">
        <div class="filter-row">
          <div class="filter-group">
            <label>Buscar:</label>
            <input
              type="text"
              class="form-control"
              [(ngModel)]="searchTerm"
              (input)="onSearch()"
              placeholder="Buscar por nombre..."
            />
          </div>
          <div class="filter-group">
            <label>Categoría:</label>
            <select class="form-control" [(ngModel)]="selectedCategory" (change)="onSearch()">
              <option value="">Todas las categorías</option>
              <option *ngFor="let cat of categories" [value]="cat">{{ cat }}</option>
            </select>
          </div>
        </div>
      </div>

      <div class="alert alert-danger" *ngIf="error">{{ error }}</div>

      <div *ngIf="loading" class="text-center">
        <div class="spinner"></div>
        <p>Cargando productos...</p>
      </div>

      <div *ngIf="!loading && productos.length === 0" class="alert alert-info">
        No se encontraron productos.
      </div>

      <div class="table-container card" *ngIf="!loading && productos.length > 0">
        <table class="table">
          <thead>
            <tr>
              <th>Nombre</th>
              <th>Descripción</th>
              <th>Categoría</th>
              <th>Precio</th>
              <th>Cantidad</th>
              <th>Estado</th>
              <th>Acciones</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let producto of productos">
              <td>{{ producto.name }}</td>
              <td>{{ producto.description | slice:0:50 }}{{ producto.description.length > 50 ? '...' : '' }}</td>
              <td>{{ producto.category }}</td>
              <td>\${{ producto.price | number:'1.2-2' }}</td>
              <td [class.low-stock]="producto.isLowStock">{{ producto.quantity }}</td>
              <td>
                <span class="badge" [class.badge-danger]="producto.isLowStock" [class.badge-success]="!producto.isLowStock">
                  {{ producto.isLowStock ? 'Bajo Stock' : 'Normal' }}
                </span>
              </td>
              <td>
                <button
                  *ngIf="producto.isLowStock && !authService.isAdmin()"
                  class="btn btn-warning btn-sm"
                  (click)="reportLowStock(producto)"
                  [disabled]="producto.reporting"
                  title="Reportar stock bajo a administradores">
                  {{ producto.reporting ? 'Enviando...' : 'Reportar Stock' }}
                </button>
                <a *ngIf="authService.isAdmin()" [routerLink]="['/productos/editar', producto.id]" class="btn btn-primary btn-sm">Editar</a>
                <button *ngIf="authService.isAdmin()" class="btn btn-danger btn-sm" (click)="deleteProduct(producto)">Eliminar</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  `,
  styles: [`
    .productos-container {
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

    .filters {
      margin-bottom: 20px;
    }

    .filter-row {
      display: flex;
      gap: 20px;
      flex-wrap: wrap;
    }

    .filter-group {
      flex: 1;
      min-width: 200px;
    }

    .filter-group label {
      display: block;
      margin-bottom: 5px;
      font-weight: 500;
    }

    .table-container {
      overflow-x: auto;
    }

    .btn-sm {
      padding: 5px 10px;
      font-size: 12px;
      margin-right: 5px;
    }

    .low-stock {
      color: #e74c3c;
      font-weight: bold;
    }

    .text-center {
      text-align: center;
      padding: 40px;
    }
  `]
})
export class ProductosListComponent implements OnInit {
  productos: Product[] = [];
  categories: string[] = [];
  searchTerm = '';
  selectedCategory = '';
  loading = false;
  error = '';

  constructor(
    private productosService: ProductosService,
    public authService: AuthService
  ) {}

  ngOnInit(): void {
    this.loadProducts();
    this.loadCategories();
  }

  loadProducts(): void {
    this.loading = true;
    this.error = '';

    this.productosService.getProducts(this.searchTerm, this.selectedCategory).subscribe({
      next: (response) => {
        this.loading = false;
        if (response.success) {
          this.productos = response.data;
        } else {
          this.error = response.message;
        }
      },
      error: (err) => {
        this.loading = false;
        this.error = err.error?.message || 'Error al cargar productos';
      }
    });
  }

  loadCategories(): void {
    this.productosService.getCategories().subscribe({
      next: (response) => {
        if (response.success) {
          this.categories = response.data;
        }
      }
    });
  }

  onSearch(): void {
    this.loadProducts();
  }

  deleteProduct(producto: Product): void {
    if (confirm(`¿Estás seguro de eliminar "${producto.name}"?`)) {
      this.productosService.deleteProduct(producto.id).subscribe({
        next: (response) => {
          if (response.success) {
            this.loadProducts();
          } else {
            this.error = response.message;
          }
        },
        error: (err) => {
          this.error = err.error?.message || 'Error al eliminar producto';
        }
      });
    }
  }

  reportLowStock(producto: any): void {
    producto.reporting = true;
    this.productosService.reportLowStock(producto.id).subscribe({
      next: (response) => {
        producto.reporting = false;
        if (response.success) {
          alert('Reporte enviado exitosamente a los administradores');
        } else {
          this.error = response.message;
        }
      },
      error: (err) => {
        producto.reporting = false;
        this.error = err.error?.message || 'Error al enviar reporte';
      }
    });
  }
}

import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';
import { ProductosService, ProductCreate, ProductUpdate } from '../services/productos.service';

@Component({
  selector: 'app-producto-form',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  template: `
    <div class="form-container">
      <div class="card">
        <div class="card-header">
          <h3 class="card-title">{{ isEditing ? 'Editar Producto' : 'Nuevo Producto' }}</h3>
        </div>

        <div class="alert alert-danger" *ngIf="error">{{ error }}</div>
        <div class="alert alert-success" *ngIf="success">{{ success }}</div>

        <form (ngSubmit)="onSubmit()" #productForm="ngForm">
          <div class="form-group">
            <label for="name">Nombre *</label>
            <input
              type="text"
              id="name"
              class="form-control"
              [(ngModel)]="product.name"
              name="name"
              required
              maxlength="200"
              placeholder="Nombre del producto"
            />
          </div>

          <div class="form-group">
            <label for="description">Descripción</label>
            <textarea
              id="description"
              class="form-control"
              [(ngModel)]="product.description"
              name="description"
              rows="3"
              maxlength="1000"
              placeholder="Descripción del producto"
            ></textarea>
          </div>

          <div class="form-row">
            <div class="form-group">
              <label for="price">Precio *</label>
              <input
                type="number"
                id="price"
                class="form-control"
                [(ngModel)]="product.price"
                name="price"
                required
                min="0.01"
                step="0.01"
                placeholder="0.00"
              />
            </div>

            <div class="form-group">
              <label for="quantity">Cantidad *</label>
              <input
                type="number"
                id="quantity"
                class="form-control"
                [(ngModel)]="product.quantity"
                name="quantity"
                required
                min="0"
                placeholder="0"
              />
            </div>
          </div>

          <div class="form-group">
            <label for="category">Categoría *</label>
            <input
              type="text"
              id="category"
              class="form-control"
              [(ngModel)]="product.category"
              name="category"
              required
              maxlength="100"
              placeholder="Ej: Electrónica, Accesorios, etc."
              list="categoryList"
            />
            <datalist id="categoryList">
              <option *ngFor="let cat of categories" [value]="cat"></option>
            </datalist>
          </div>

          <div class="alert alert-warning" *ngIf="product.quantity < 5 && product.quantity >= 0">
            Advertencia: La cantidad es menor a 5. Se notificará a los administradores sobre el bajo inventario.
          </div>

          <div class="form-actions">
            <a routerLink="/productos" class="btn btn-secondary">Cancelar</a>
            <button type="submit" class="btn btn-success" [disabled]="loading || !productForm.valid">
              <span *ngIf="loading">Guardando...</span>
              <span *ngIf="!loading">{{ isEditing ? 'Actualizar' : 'Crear' }} Producto</span>
            </button>
          </div>
        </form>
      </div>
    </div>
  `,
  styles: [`
    .form-container {
      max-width: 600px;
      margin: 20px auto;
    }

    .form-row {
      display: flex;
      gap: 20px;
    }

    .form-row .form-group {
      flex: 1;
    }

    textarea.form-control {
      resize: vertical;
    }

    .form-actions {
      display: flex;
      justify-content: flex-end;
      gap: 10px;
      margin-top: 20px;
      padding-top: 20px;
      border-top: 1px solid #eee;
    }
  `]
})
export class ProductoFormComponent implements OnInit {
  product: ProductCreate = {
    name: '',
    description: '',
    price: 0,
    quantity: 0,
    category: ''
  };

  categories: string[] = [];
  isEditing = false;
  productId: number | null = null;
  loading = false;
  error = '';
  success = '';

  constructor(
    private productosService: ProductosService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.loadCategories();

    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditing = true;
      this.productId = parseInt(id, 10);
      this.loadProduct();
    }
  }

  loadProduct(): void {
    if (!this.productId) return;

    this.loading = true;
    this.productosService.getProductById(this.productId).subscribe({
      next: (response) => {
        this.loading = false;
        if (response.success) {
          const p = response.data;
          this.product = {
            name: p.name,
            description: p.description,
            price: p.price,
            quantity: p.quantity,
            category: p.category
          };
        } else {
          this.error = response.message;
        }
      },
      error: (err) => {
        this.loading = false;
        this.error = err.error?.message || 'Error al cargar producto';
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

  onSubmit(): void {
    this.loading = true;
    this.error = '';
    this.success = '';

    const request = this.isEditing
      ? this.productosService.updateProduct(this.productId!, this.product as ProductUpdate)
      : this.productosService.createProduct(this.product);

    request.subscribe({
      next: (response) => {
        this.loading = false;
        if (response.success) {
          this.success = this.isEditing ? 'Producto actualizado exitosamente' : 'Producto creado exitosamente';
          setTimeout(() => {
            this.router.navigate(['/productos']);
          }, 1500);
        } else {
          this.error = response.message;
        }
      },
      error: (err) => {
        this.loading = false;
        this.error = err.error?.message || 'Error al guardar producto';
      }
    });
  }
}

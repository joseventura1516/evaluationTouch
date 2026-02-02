import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService, RegisterRequest } from '../services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  template: `
    <div class="auth-container">
      <div class="auth-card">
        <h2>Crear Cuenta</h2>
        <p class="subtitle">Sistema de Gestión de Inventario</p>

        <div class="alert alert-danger" *ngIf="error">{{ error }}</div>
        <div class="alert alert-success" *ngIf="success">{{ success }}</div>

        <form (ngSubmit)="register()" #registerForm="ngForm">
          <div class="form-group">
            <label for="username">Nombre de Usuario</label>
            <input
              type="text"
              id="username"
              class="form-control"
              [(ngModel)]="userData.username"
              name="username"
              required
              minlength="3"
              placeholder="Usuario"
            />
          </div>

          <div class="form-group">
            <label for="email">Email</label>
            <input
              type="email"
              id="email"
              class="form-control"
              [(ngModel)]="userData.email"
              name="email"
              required
              email
              placeholder="correo@ejemplo.com"
            />
          </div>

          <div class="form-group">
            <label for="password">Contraseña</label>
            <input
              type="password"
              id="password"
              class="form-control"
              [(ngModel)]="userData.password"
              name="password"
              required
              minlength="6"
              placeholder="Mínimo 6 caracteres"
            />
          </div>

          <div class="form-group">
            <label for="role">Rol</label>
            <select
              id="role"
              class="form-control"
              [(ngModel)]="userData.role"
              name="role"
              required
            >
              <option value="Empleado">Empleado</option>
              <option value="Administrador">Administrador</option>
            </select>
          </div>

          <button type="submit" class="btn btn-success btn-block" [disabled]="loading || !registerForm.valid">
            <span *ngIf="loading">Registrando...</span>
            <span *ngIf="!loading">Crear Cuenta</span>
          </button>
        </form>

        <p class="auth-link">
          ¿Ya tienes cuenta? <a routerLink="/login">Inicia sesión</a>
        </p>
      </div>
    </div>
  `,
  styles: [`
    .auth-container {
      min-height: 100vh;
      display: flex;
      align-items: center;
      justify-content: center;
      background: linear-gradient(135deg, #2c3e50 0%, #27ae60 100%);
      padding: 20px;
    }

    .auth-card {
      background: white;
      padding: 40px;
      border-radius: 10px;
      box-shadow: 0 10px 30px rgba(0, 0, 0, 0.2);
      width: 100%;
      max-width: 400px;
    }

    h2 {
      text-align: center;
      color: #2c3e50;
      margin-bottom: 5px;
    }

    .subtitle {
      text-align: center;
      color: #7f8c8d;
      margin-bottom: 30px;
    }

    .btn-block {
      width: 100%;
      margin-top: 20px;
    }

    .auth-link {
      text-align: center;
      margin-top: 20px;
      color: #7f8c8d;
    }

    .auth-link a {
      color: #27ae60;
      text-decoration: none;
    }

    .auth-link a:hover {
      text-decoration: underline;
    }
  `]
})
export class RegisterComponent {
  userData: RegisterRequest = {
    username: '',
    email: '',
    password: '',
    role: 'Empleado'
  };
  loading = false;
  error = '';
  success = '';

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  register(): void {
    this.loading = true;
    this.error = '';
    this.success = '';

    this.authService.register(this.userData).subscribe({
      next: (response) => {
        this.loading = false;
        if (response.success) {
          this.success = 'Cuenta creada exitosamente. Redirigiendo...';
          setTimeout(() => {
            this.router.navigate(['/login']);
          }, 2000);
        } else {
          this.error = response.message;
        }
      },
      error: (err) => {
        this.loading = false;
        this.error = err.error?.message || 'Error al registrar usuario';
      }
    });
  }
}

import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService, LoginRequest } from '../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  template: `
    <div class="auth-container">
      <div class="auth-card">
        <h2>Iniciar Sesión</h2>
        <p class="subtitle">Sistema de Gestión de Inventario</p>

        <div class="alert alert-danger" *ngIf="error">{{ error }}</div>

        <form (ngSubmit)="login()" #loginForm="ngForm">
          <div class="form-group">
            <label for="email">Email</label>
            <input
              type="email"
              id="email"
              class="form-control"
              [(ngModel)]="credentials.email"
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
              [(ngModel)]="credentials.password"
              name="password"
              required
              minlength="6"
              placeholder="••••••••"
            />
          </div>

          <button type="submit" class="btn btn-primary btn-block" [disabled]="loading || !loginForm.valid">
            <span *ngIf="loading">Cargando...</span>
            <span *ngIf="!loading">Ingresar</span>
          </button>
        </form>

        <p class="auth-link">
          ¿No tienes cuenta? <a routerLink="/register">Regístrate aquí</a>
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
      background: linear-gradient(135deg, #2c3e50 0%, #3498db 100%);
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
      color: #3498db;
      text-decoration: none;
    }

    .auth-link a:hover {
      text-decoration: underline;
    }
  `]
})
export class LoginComponent {
  credentials: LoginRequest = {
    email: '',
    password: ''
  };
  loading = false;
  error = '';

  constructor(
    private authService: AuthService,
    private router: Router
  ) {
    if (this.authService.isLoggedIn()) {
      this.router.navigate(['/productos']);
    }
  }

  login(): void {
    this.loading = true;
    this.error = '';

    this.authService.login(this.credentials).subscribe({
      next: (response) => {
        this.loading = false;
        if (response.success) {
          this.router.navigate(['/productos']);
        } else {
          this.error = response.message;
        }
      },
      error: (err) => {
        this.loading = false;
        this.error = err.error?.message || 'Error al iniciar sesión';
      }
    });
  }
}

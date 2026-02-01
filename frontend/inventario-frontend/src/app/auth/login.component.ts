import { Component } from '@angular/core';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-login',
  template: `
    <h2>Login</h2>
    <form (ngSubmit)="login()">
      <input [(ngModel)]="username" name="username" placeholder="Usuario" />
      <input [(ngModel)]="password" name="password" type="password" placeholder="Contraseña" />
      <button type="submit">Ingresar</button>
    </form>
  `
})
export class LoginComponent {
  username = '';
  password = '';

  constructor(private authService: AuthService) {}

  login() {
    this.authService.login(this.username, this.password).subscribe(res => {
      localStorage.setItem('token', res.token);
      localStorage.setItem('rol', res.rol);
      alert('Login exitoso');
    });
  }
}
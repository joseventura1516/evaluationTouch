import { Routes } from '@angular/router';
import { authGuard } from './guards/auth.guard';
import { adminGuard } from './guards/admin.guard';

export const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  {
    path: 'login',
    loadComponent: () => import('./auth/login.component').then(m => m.LoginComponent)
  },
  {
    path: 'register',
    loadComponent: () => import('./auth/register.component').then(m => m.RegisterComponent)
  },
  {
    path: 'productos',
    loadComponent: () => import('./productos/productos-list.component').then(m => m.ProductosListComponent),
    canActivate: [authGuard]
  },
  {
    path: 'productos/nuevo',
    loadComponent: () => import('./productos/producto-form.component').then(m => m.ProductoFormComponent),
    canActivate: [authGuard, adminGuard]
  },
  {
    path: 'productos/editar/:id',
    loadComponent: () => import('./productos/producto-form.component').then(m => m.ProductoFormComponent),
    canActivate: [authGuard, adminGuard]
  },
  {
    path: 'reportes',
    loadComponent: () => import('./reportes/reportes.component').then(m => m.ReportesComponent),
    canActivate: [authGuard, adminGuard]
  },
  {
    path: 'notificaciones',
    loadComponent: () => import('./notificaciones/notificaciones.component').then(m => m.NotificacionesComponent),
    canActivate: [authGuard, adminGuard]
  },
  { path: '**', redirectTo: '/login' }
];

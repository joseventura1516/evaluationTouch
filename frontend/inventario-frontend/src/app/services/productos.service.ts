import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({ providedIn: 'root' })
export class ProductosService {
  private apiUrl = 'http://localhost:5000/api/productos';

  constructor(private http: HttpClient) {}

  getProductos() {
    return this.http.get<any[]>(this.apiUrl);
  }

  crearProducto(producto: any) {
    return this.http.post(this.apiUrl, producto);
  }
}
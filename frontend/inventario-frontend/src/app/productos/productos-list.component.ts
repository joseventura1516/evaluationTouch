import { Component, OnInit } from '@angular/core';
import { ProductosService } from '../services/productos.service';

@Component({
  selector: 'app-productos-list',
  template: `
    <h2>Lista de Productos</h2>
    <ul>
      <li *ngFor="let p of productos">
        {{p.nombre}} - {{p.cantidad}} unidades
      </li>
    </ul>
  `
})
export class ProductosListComponent implements OnInit {
  productos: any[] = [];

  constructor(private productosService: ProductosService) {}

  ngOnInit() {
    this.productosService.getProductos().subscribe(data => this.productos = data);
  }
}
using Microsoft.AspNetCore.Mvc;
using InventarioAPI.Data;
using InventarioAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace InventarioAPI.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : ControllerBase {
        private readonly AppDbContext _context;

        public ProductosController(AppDbContext context) {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetProductos() {
            return Ok(await _context.Productos.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> CrearProducto(Producto producto) {
            if (producto.Cantidad < 0) return BadRequest("Cantidad no puede ser negativa");
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();
            return Ok(producto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarProducto(int id, Producto producto) {
            var prod = await _context.Productos.FindAsync(id);
            if (prod == null) return NotFound();
            prod.Nombre = producto.Nombre;
            prod.Descripcion = producto.Descripcion;
            prod.Precio = producto.Precio;
            prod.Cantidad = producto.Cantidad;
            prod.Categoria = producto.Categoria;
            await _context.SaveChangesAsync();
            return Ok(prod);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarProducto(int id) {
            var prod = await _context.Productos.FindAsync(id);
            if (prod == null) return NotFound();
            _context.Productos.Remove(prod);
            await _context.SaveChangesAsync();
            return Ok("Producto eliminado");
        }
    }
}
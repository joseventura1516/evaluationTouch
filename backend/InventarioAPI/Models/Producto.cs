using System.ComponentModel.DataAnnotations;

namespace InventarioAPI.Models {
    public class Producto {
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        public string Descripcion { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El precio no puede ser negativo")]
        public decimal Precio { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "La cantidad no puede ser negativa")]
        public int Cantidad { get; set; }

        public string Categoria { get; set; }
    }
}
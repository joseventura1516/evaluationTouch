using System.ComponentModel.DataAnnotations;

namespace InventarioAPI.Models {
    public class Usuario {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public string Rol { get; set; } // "Administrador" o "Empleado"
    }
}
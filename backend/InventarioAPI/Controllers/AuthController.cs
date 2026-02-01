using Microsoft.AspNetCore.Mvc;
using InventarioAPI.Models;
using InventarioAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace InventarioAPI.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context) {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(Usuario usuario) {
            // Aquí deberías encriptar la contraseña con BCrypt
            usuario.PasswordHash = usuario.PasswordHash; 
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return Ok("Usuario registrado");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Usuario login) {
            var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Username == login.Username);
            if (user == null) return Unauthorized("Usuario no encontrado");

            // Aquí deberías validar el hash de la contraseña
            if (user.PasswordHash != login.PasswordHash) return Unauthorized("Contraseña incorrecta");

            // Generar token JWT (simplificado aquí)
            return Ok(new { token = "FAKE-JWT-TOKEN", rol = user.Rol });
        }
    }
}
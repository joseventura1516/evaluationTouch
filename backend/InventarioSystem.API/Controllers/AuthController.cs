using InventarioSystem.Core.DTOs;
using InventarioSystem.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InventarioSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _authService.RegisterAsync(registerDto);

                if (!result.Success)
                    return BadRequest(new { message = result.Message });

                return Ok(new { success = true, message = "Usuario registrado exitosamente", data = result.Data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en registro de usuario");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _authService.LoginAsync(loginDto);

                if (!result.Success)
                    return Unauthorized(new { message = result.Message });

                return Ok(new { success = true, message = "Login exitoso", data = result.Data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en login");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}

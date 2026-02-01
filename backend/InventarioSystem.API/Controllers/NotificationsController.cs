using InventarioSystem.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InventarioSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Administrador")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<NotificationsController> _logger;

        public NotificationsController(INotificationService notificationService, ILogger<NotificationsController> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserNotifications()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var notifications = await _notificationService.GetUserNotificationsAsync(userId);

                return Ok(new { data = notifications });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo notificaciones");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var count = await _notificationService.GetUnreadCountAsync(userId);

                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo conteo de notificaciones");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPut("{id}/mark-read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            try
            {
                var result = await _notificationService.MarkAsReadAsync(id);

                if (!result)
                    return NotFound(new { message = "Notificación no encontrada" });

                return Ok(new { message = "Notificación marcada como leída" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marcando notificación como leída");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}

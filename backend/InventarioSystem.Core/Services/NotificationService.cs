using InventarioSystem.Core.DTOs;
using InventarioSystem.Core.Entities;
using InventarioSystem.Core.Interfaces;

namespace InventarioSystem.Core.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IUserRepository _userRepository;

        public NotificationService(INotificationRepository notificationRepository, IUserRepository userRepository)
        {
            _notificationRepository = notificationRepository;
            _userRepository = userRepository;
        }

        public async Task<ServiceResponse<IEnumerable<NotificationDto>>> GetNotificationsByUserIdAsync(int userId)
        {
            var notifications = await _notificationRepository.GetByUserIdAsync(userId);
            var notificationDtos = notifications.Select(MapToDto);
            return ServiceResponse<IEnumerable<NotificationDto>>.SuccessResponse(notificationDtos);
        }

        public async Task<ServiceResponse<IEnumerable<NotificationDto>>> GetUnreadNotificationsByUserIdAsync(int userId)
        {
            var notifications = await _notificationRepository.GetUnreadByUserIdAsync(userId);
            var notificationDtos = notifications.Select(MapToDto);
            return ServiceResponse<IEnumerable<NotificationDto>>.SuccessResponse(notificationDtos);
        }

        public async Task<ServiceResponse<int>> GetUnreadCountAsync(int userId)
        {
            var count = await _notificationRepository.GetUnreadCountByUserIdAsync(userId);
            return ServiceResponse<int>.SuccessResponse(count);
        }

        public async Task<ServiceResponse<NotificationDto>> MarkAsReadAsync(int notificationId, int userId)
        {
            var notification = await _notificationRepository.GetByIdAsync(notificationId);

            if (notification == null)
            {
                return ServiceResponse<NotificationDto>.FailureResponse("Notificación no encontrada");
            }

            if (notification.UserId != userId)
            {
                return ServiceResponse<NotificationDto>.FailureResponse("No tienes permiso para modificar esta notificación");
            }

            var updatedNotification = await _notificationRepository.MarkAsReadAsync(notificationId);
            return ServiceResponse<NotificationDto>.SuccessResponse(MapToDto(updatedNotification), "Notificación marcada como leída");
        }

        public async Task<ServiceResponse<bool>> MarkAllAsReadAsync(int userId)
        {
            await _notificationRepository.MarkAllAsReadByUserIdAsync(userId);
            return ServiceResponse<bool>.SuccessResponse(true, "Todas las notificaciones marcadas como leídas");
        }

        public async Task<ServiceResponse<NotificationDto>> CreateNotificationAsync(int userId, string message, string type = "Info")
        {
            var notification = new Notification
            {
                UserId = userId,
                Message = message,
                Type = type,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            var createdNotification = await _notificationRepository.CreateAsync(notification);
            return ServiceResponse<NotificationDto>.SuccessResponse(MapToDto(createdNotification), "Notificación creada exitosamente");
        }

        public async Task NotifyAdministratorsAsync(string message, string type = "Warning")
        {
            var administrators = await _userRepository.GetAdministratorsAsync();

            foreach (var admin in administrators)
            {
                var notification = new Notification
                {
                    UserId = admin.Id,
                    Message = message,
                    Type = type,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };

                await _notificationRepository.CreateAsync(notification);
            }
        }

        private static NotificationDto MapToDto(Notification notification)
        {
            return new NotificationDto
            {
                Id = notification.Id,
                Message = notification.Message,
                Type = notification.Type,
                IsRead = notification.IsRead,
                CreatedAt = notification.CreatedAt
            };
        }
    }
}

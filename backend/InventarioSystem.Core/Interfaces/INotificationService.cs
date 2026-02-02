using InventarioSystem.Core.DTOs;

namespace InventarioSystem.Core.Interfaces
{
    public interface INotificationService
    {
        Task<ServiceResponse<IEnumerable<NotificationDto>>> GetNotificationsByUserIdAsync(int userId);
        Task<ServiceResponse<IEnumerable<NotificationDto>>> GetUnreadNotificationsByUserIdAsync(int userId);
        Task<ServiceResponse<int>> GetUnreadCountAsync(int userId);
        Task<ServiceResponse<NotificationDto>> MarkAsReadAsync(int notificationId, int userId);
        Task<ServiceResponse<bool>> MarkAllAsReadAsync(int userId);
        Task<ServiceResponse<NotificationDto>> CreateNotificationAsync(int userId, string message, string type = "Info");
        Task NotifyAdministratorsAsync(string message, string type = "Warning");
    }
}

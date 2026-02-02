using InventarioSystem.Core.Entities;

namespace InventarioSystem.Core.Interfaces
{
    public interface INotificationRepository
    {
        Task<Notification?> GetByIdAsync(int id);
        Task<IEnumerable<Notification>> GetByUserIdAsync(int userId);
        Task<IEnumerable<Notification>> GetUnreadByUserIdAsync(int userId);
        Task<int> GetUnreadCountByUserIdAsync(int userId);
        Task<Notification> CreateAsync(Notification notification);
        Task<Notification> MarkAsReadAsync(int id);
        Task<bool> MarkAllAsReadByUserIdAsync(int userId);
        Task<bool> DeleteAsync(int id);
    }
}

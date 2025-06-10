using YouNGU.Models.Entities;

namespace YouNGU.Data.Repositories
{
    public interface INotificationService
    {
        Task<List<Notification>> GetNotificationsForUserAsync(string userId, int count = 10);
        Task<int> GetUnreadNotificationCountAsync(string userId);
        Task CreateNotificationAsync(Notification notification);
        Task MarkAsReadAsync(int notificationId);
        Task MarkAllAsReadAsync(string userId);
        Task DeleteNotificationAsync(int notificationId);
    }
}

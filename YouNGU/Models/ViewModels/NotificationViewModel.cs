using YouNGU.Models.Entities;

namespace YouNGU.Models.ViewModels
{
    public class NotificationViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string Link { get; set; }
        public bool IsRead { get; set; }
        public NotificationType Type { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}

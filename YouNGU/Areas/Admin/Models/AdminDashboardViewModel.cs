using YouNGU.Models.Entities;

namespace YouNGU.Areas.Admin.Models
{
    public class AdminDashboardViewModel
    {
        public int TotalVideos { get; set; }
        public int TotalUsers { get; set; }
        public IEnumerable<Video> RecentVideos { get; set; }
        public IEnumerable<ApplicationUser> RecentUsers { get; set; }
        public double VideoChangePercentage { get; set; } // Phần trăm thay đổi so với kỳ trước
        public double UserChangePercentage { get; set; }
        public string TotalViewsFormatted { get; set; } = "0"; // Ví dụ: "2.3M"
        public double ViewChangePercentage { get; set; }
        public string StorageUsedFormatted { get; set; } = "0GB"; // Ví dụ: "450GB"
        public double StorageUsagePercentage { get; set; } // Ví dụ: 75
    }
    public class RecentUserViewModel
    {
        public string Id { get; set; } = "N/A";
        public string UserName { get; set; } = "N/A";
        public string Email { get; set; } = "N/A";
        public string? AvatarUrl { get; set; }
        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
        public string Role { get; set; } = "N/A"; // Ví dụ: "Admin", "Người dùng"
        public string Status { get; set; } = "Hoạt động"; // Ví dụ: "Hoạt động", "Bị khóa"

    }
    public class RecentVideoViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = "N/A";
        public string? ThumbnailUrl { get; set; } // Có thể là URL từ Cloudinary hoặc local path đã xử lý
        public string UploaderName { get; set; } = "N/A";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "N/A"; // Ví dụ: "Đã xuất bản", "Đang xử lý"
        public long ViewCount { get; set; } = 0;
        public string? VideoUrl { get; set; } // Có thể là URL từ Cloudinary hoặc local path đã xử lý
        public string Description { get; set; } = "N/A";
        // Thêm UserId nếu cần link đến trang người dùng/kênh
        public string? UserId { get; set; }
    }

}

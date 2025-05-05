using YouNGU.Models.Entities;

namespace YouNGU.Models.ViewModels
{
    public class VideoDetailViewModel
    {
        public Video Video { get; set; }
        public bool UserLiked { get; set; }
        public bool IsSubscribed { get; set; }
        public int SubscriberCount { get; set; }
        public IEnumerable<Comment> Comments { get; set; }
        public IEnumerable<Video> RecommendedVideos { get; set; }

    }
    public static class FormatHelpers
    {
        public static string FormatDuration(TimeSpan? duration)
        {
            if (!duration.HasValue || duration.Value.TotalSeconds <= 0) return "0:00";
            var d = duration.Value;
            if (d.TotalHours >= 1)
            {
                return $"{(int)d.TotalHours}:{d.Minutes:D2}:{d.Seconds:D2}";
            }
            else
            {
                // Hiển thị 00:ss nếu dưới 1 phút
                if (d.TotalMinutes < 1) return $"0:{d.Seconds:D2}";
                // Hiển thị mm:ss nếu từ 1 phút trở lên
                return $"{d.Minutes}:{d.Seconds:D2}";
            }
        }

        public static string TimeAgo(DateTime dateTime)
        {
            var timeSpan = DateTime.UtcNow - dateTime;

            if (timeSpan.TotalSeconds < 60) return $"{(int)timeSpan.TotalSeconds} giây trước";
            if (timeSpan.TotalMinutes < 60) return $"{(int)timeSpan.TotalMinutes} phút trước";
            if (timeSpan.TotalHours < 24) return $"{(int)timeSpan.TotalHours} giờ trước";
            if (timeSpan.TotalDays < 7) return $"{(int)timeSpan.TotalDays} ngày trước";
            if (timeSpan.TotalDays < 30) return $"{(int)(timeSpan.TotalDays / 7)} tuần trước";
            if (timeSpan.TotalDays < 365) return $"{(int)(timeSpan.TotalDays / 30)} tháng trước";
            return $"{(int)(timeSpan.TotalDays / 365)} năm trước";
        }
    }
}

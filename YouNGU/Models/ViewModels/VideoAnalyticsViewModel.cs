using YouNGU.Models.Entities;

namespace YouNGU.Models.ViewModels
{
    public class VideoAnalyticsViewModel
    {
        public Video Video { get; set; }
        public VideoAnalytics Analytics { get; set; }
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
    }
    public class VideoAnalytics
    {
        public string PublicId { get; set; }
        public long Views { get; set; }
        public long Bandwidth { get; set; }
        public DateTime? LastAccessed { get; set; }
        public string AccessMode { get; set; }
    }
}

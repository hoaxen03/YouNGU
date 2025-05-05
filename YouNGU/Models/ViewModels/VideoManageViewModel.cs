using YouNGU.Models.Entities;

namespace YouNGU.Models.ViewModels
{
    public class VideoManageViewModel
    {
        public IEnumerable<VideoWithAnalytics> Videos { get; set; }
        public int TotalVideos { get; set; }
        public long TotalViews { get; set; }
        public long TotalLikes { get; set; }
        public long TotalComments { get; set; }
    }

    public class VideoWithAnalytics
    {
        public Video Video { get; set; }
        public VideoAnalytics Analytics { get; set; }
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
    }

}

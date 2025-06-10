using YouNGU.Models.Entities;

namespace YouNGU.Areas.Admin.Models
{
    public class AdminDashboardViewModel
    {
        public int TotalVideos { get; set; }
        public int TotalUsers { get; set; }
        public IList<Video> RecentVideos { get; set; } = new List<Video>();
        public IList<ApplicationUser> RecentUsers { get; set; } = new List<ApplicationUser>();
        public Dictionary<VideoStatus, int> VideoStatusCounts { get; set; } = new Dictionary<VideoStatus, int>();

    }


}

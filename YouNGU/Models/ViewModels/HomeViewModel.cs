using YouNGU.Models.Entities;

namespace YouNGU.Models.ViewModels
{
    public class HomeViewModel
    {
        public IEnumerable<Video> RecentVideos { get; set; }
        public IEnumerable<Video> PopularVideos { get; set; }
        public IEnumerable<Category> Categories { get; set; }

    }
}

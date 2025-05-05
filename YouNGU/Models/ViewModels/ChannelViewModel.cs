using YouNGU.Models.Entities;

namespace YouNGU.Models.ViewModels
{
    public class ChannelViewModel
    {
        public ApplicationUser User { get; set; }
        public IEnumerable<Video> Videos { get; set; }
        public int SubscriberCount { get; set; }
        public bool IsSubscribed { get; set; }
        public IEnumerable<Playlist> PublicPlaylists { get; set; }
    }
}

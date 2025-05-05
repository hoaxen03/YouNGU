using YouNGU.Models.Entities;

namespace YouNGU.Models.ViewModels
{
    public class PlaylistViewModel
    {
        public Playlist Playlist { get; set; }
        public IEnumerable<Video> Videos { get; set; }
        public bool IsOwner { get; set; }

    }
}

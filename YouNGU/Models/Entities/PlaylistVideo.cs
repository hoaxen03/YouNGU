namespace YouNGU.Models.Entities
{
    public class PlaylistVideo
    {
        public int Id { get; set; }
        public int PlaylistId { get; set; }
        public int VideoId { get; set; }
        public int Order { get; set; }

        // Navigation properties
        public Playlist Playlist { get; set; }
        public Video Video { get; set; }

    }
}

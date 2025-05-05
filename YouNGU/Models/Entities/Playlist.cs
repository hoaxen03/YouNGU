namespace YouNGU.Models.Entities
{
    public class Playlist
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
        public bool IsPublic { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public ApplicationUser User { get; set; }
        public ICollection<PlaylistVideo> PlaylistVideos { get; set; }
    }
}

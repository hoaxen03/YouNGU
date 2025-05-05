namespace YouNGU.Models.Entities
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Navigation properties
        public ICollection<VideoTag> VideoTags { get; set; }

    }
}

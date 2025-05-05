namespace YouNGU.Models.Entities
{
    public class VideoTag
    {
        public int VideoId { get; set; }
        public int TagId { get; set; }

        // Navigation properties
        public Video Video { get; set; }
        public Tag Tag { get; set; }

    }
}

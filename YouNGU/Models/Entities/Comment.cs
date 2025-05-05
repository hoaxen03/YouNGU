namespace YouNGU.Models.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int VideoId { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public Video Video { get; set; }
        public ApplicationUser User { get; set; }
    }
}

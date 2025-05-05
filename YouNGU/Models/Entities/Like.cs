namespace YouNGU.Models.Entities
{
    public class Like
    {
        public int Id { get; set; }
        public int VideoId { get; set; }
        public string UserId { get; set; }
        public bool IsLike { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public Video Video { get; set; }
        public ApplicationUser User { get; set; }

    }
}

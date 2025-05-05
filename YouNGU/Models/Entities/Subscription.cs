namespace YouNGU.Models.Entities
{
    public class Subscription
    {
        public int Id { get; set; }
        public string SubscriberId { get; set; }
        public string PublisherId { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public ApplicationUser Subscriber { get; set; }
        public ApplicationUser Publisher { get; set; }

    }
}

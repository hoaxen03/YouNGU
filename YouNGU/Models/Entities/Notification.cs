using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace YouNGU.Models.Entities;

public class Notification
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; }

    [Required]
    public string Title { get; set; }

    public string Message { get; set; }

    public string Link { get; set; }

    public NotificationType Type { get; set; }

    public bool IsRead { get; set; }

    public DateTime CreatedAt { get; set; }

    [ForeignKey("UserId")]
    public virtual ApplicationUser User { get; set; }

    // Tham chiếu tùy chọn đến đối tượng liên quan (video, bình luận, v.v.)
    public int? RelatedVideoId { get; set; }
    public int? RelatedCommentId { get; set; }

    [ForeignKey("RelatedVideoId")]
    public virtual Video RelatedVideo { get; set; }
}

public enum NotificationType
{
    Interaction = 0,
    Subscription = 1,
    NewContent = 2,
    System = 3,
    Achievement = 4
}

using System.Collections.Generic;
namespace YouNGU.Models.Entities
{
    // Models/Entities/Video.cs
    public class Video
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        // Thông tin Cloudinary cho video
        public string CloudinaryPublicId { get; set; }
        public string CloudinaryUrl { get; set; }
        public string StreamingProfile { get; set; }
        public TimeSpan Duration { get; set; }

        // Thông tin Cloudinary cho thumbnail
        public string ThumbnailPublicId { get; set; }
        public string ThumbnailUrl { get; set; }

        public string VideoPath { get; set; }
        public string ThumbnailPath { get; set; }


        public string UserId { get; set; }
        public int CategoryId { get; set; }
        public int ViewCount { get; set; }
        public VideoStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public ApplicationUser User { get; set; }
        public Category Category { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Like> Likes { get; set; }
        public ICollection<VideoTag> VideoTags { get; set; }
        public ICollection<PlaylistVideo> PlaylistVideos { get; set; }
    }
    public enum VideoStatus
    {
        Processing = 0,
        Published = 1,
        Private = 2,
        Public = 3,
        Unlisted = 4,
        Deleted = 5
    }
}

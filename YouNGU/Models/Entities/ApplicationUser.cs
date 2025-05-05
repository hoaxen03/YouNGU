using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections.Generic;

namespace YouNGU.Models.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string? AvatarPath { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public ICollection<Video> Videos { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Like> Likes { get; set; }
        public ICollection<Subscription> Subscriptions { get; set; }
        public ICollection<Subscription> Subscribers { get; set; }
        public ICollection<Playlist> Playlists { get; set; }
    }
}

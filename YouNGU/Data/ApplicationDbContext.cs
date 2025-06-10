using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using YouNGU.Models;
using YouNGU.Models.Entities;

namespace YouNGU.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Video> Videos { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<PlaylistVideo> PlaylistVideos { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<VideoTag> VideoTags { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Log> Logs { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure relationships
            // Thay đổi hành vi xóa cho khóa ngoại UserId trong bảng Videos
            builder.Entity<Video>()
                .HasOne(v => v.User)
                .WithMany(u => u.Videos)
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Giữ nguyên Cascade

            // Thay đổi hành vi xóa cho khóa ngoại VideoId trong bảng Comments
            builder.Entity<Comment>()
                .HasOne(c => c.Video)
                .WithMany(v => v.Comments)
                .HasForeignKey(c => c.VideoId)
                .OnDelete(DeleteBehavior.Cascade); // Giữ nguyên Cascade

            // Thay đổi hành vi xóa cho khóa ngoại UserId trong bảng Comments
            builder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.NoAction); // Thay đổi từ Cascade thành NoAction

            // Sửa đổi từ phần trước (nếu có)
            builder.Entity<Comment>()
                .HasOne(c => c.Video)
                .WithMany(v => v.Comments)
                .HasForeignKey(c => c.VideoId)
                .OnDelete(DeleteBehavior.NoAction);

            // Thêm mới: Thay đổi hành vi xóa cho khóa ngoại VideoId trong bảng Likes
            builder.Entity<Like>()
                .HasOne(l => l.Video)
                .WithMany(v => v.Likes)
                .HasForeignKey(l => l.VideoId)
                .OnDelete(DeleteBehavior.NoAction); // Thay đổi từ Cascade thành NoAction

            // Thêm các bảng khác nếu cần
            builder.Entity<VideoTag>()
                .HasOne(vt => vt.Video)
                .WithMany(v => v.VideoTags)
                .HasForeignKey(vt => vt.VideoId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<PlaylistVideo>()
                .HasOne(pv => pv.Video)
                .WithMany(v => v.PlaylistVideos)
                .HasForeignKey(pv => pv.VideoId)
                .OnDelete(DeleteBehavior.NoAction);

            // 3. Hoặc thay đổi hành vi xóa cho các bảng phụ thuộc vào AspNetUsers
            builder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Like>()
                .HasOne(l => l.User)
                .WithMany(u => u.Likes)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.NoAction);


            builder.Entity<PlaylistVideo>()
                .HasKey(pv => new { pv.PlaylistId, pv.VideoId });

            builder.Entity<VideoTag>()
                .HasKey(vt => new { vt.VideoId, vt.TagId });

            builder.Entity<PlaylistVideo>()
                .HasOne(pv => pv.Playlist)
                .WithMany(p => p.PlaylistVideos)
                .HasForeignKey(pv => pv.PlaylistId);

            builder.Entity<PlaylistVideo>()
                .HasOne(pv => pv.Video)
                .WithMany(v => v.PlaylistVideos)
                .HasForeignKey(pv => pv.VideoId);

            builder.Entity<Subscription>()
                .HasOne(s => s.Subscriber)
                .WithMany(u => u.Subscriptions)
                .HasForeignKey(s => s.SubscriberId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Subscription>()
                .HasOne(s => s.Publisher)
                .WithMany(u => u.Subscribers)
                .HasForeignKey(s => s.PublisherId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}

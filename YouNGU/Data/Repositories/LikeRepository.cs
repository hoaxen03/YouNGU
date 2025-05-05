using Microsoft.EntityFrameworkCore;
using YouNGU.Models.Entities;

namespace YouNGU.Data.Repositories
{
    public class LikeRepository : ILikeRepository
    {
        private readonly ApplicationDbContext _context;

        public LikeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Like>> GetByVideoIdAsync(int videoId)
        {
            return await _context.Likes
                .Where(l => l.VideoId == videoId)
                .Include(l => l.User)
                .ToListAsync();
        }

        public async Task<bool> HasUserLikedVideoAsync(string userId, int videoId)
        {
            return await _context.Likes
                .AnyAsync(l => l.UserId == userId && l.VideoId == videoId && l.IsLike);
        }

        public async Task ToggleLikeAsync(int videoId, string userId)
        {
            var like = await _context.Likes
                .FirstOrDefaultAsync(l => l.VideoId == videoId && l.UserId == userId);

            if (like == null)
            {
                // Nếu chưa có like/dislike, thêm mới
                like = new Like
                {
                    VideoId = videoId,
                    UserId = userId,
                    IsLike = true,
                    CreatedAt = DateTime.UtcNow
                };
                await _context.Likes.AddAsync(like);
            }
            else
            {
                // Nếu đã có, toggle trạng thái
                like.IsLike = !like.IsLike;
                like.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<int> GetLikeCountAsync(int videoId)
        {
            return await _context.Likes
                .CountAsync(l => l.VideoId == videoId && l.IsLike);
        }
    }
}

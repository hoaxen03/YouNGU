using Microsoft.EntityFrameworkCore;
using YouNGU.Models.Entities;

namespace YouNGU.Data.Repositories
{
    public class VideoRepository : IVideoRepository
    {
        private readonly ApplicationDbContext _context;

        public VideoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Video>> GetAllAsync()
        {
            return await _context.Videos
                .Include(v => v.User)
                .Include(v => v.Category)
                .Where(v => v.Status == VideoStatus.Published)
                .OrderByDescending(v => v.CreatedAt)
                .ToListAsync();
        }

        public async Task<Video> GetByIdAsync(int id)
        {
            return await _context.Videos
                .Include(v => v.User)
                .Include(v => v.Category)
                .Include(v => v.Comments)
                    .ThenInclude(c => c.User)
                .Include(v => v.Likes)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<IEnumerable<Video>> GetByUserIdAsync(string userId)
        {
            return await _context.Videos
                .Where(v => v.UserId == userId)
                .OrderByDescending(v => v.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Video>> SearchAsync(string searchTerm)
        {
            return await _context.Videos
                .Include(v => v.User)
                .Where(v => v.Status == VideoStatus.Published &&
                           (v.Title.Contains(searchTerm) ||
                            v.Description.Contains(searchTerm)))
                .OrderByDescending(v => v.ViewCount)
                .ToListAsync();
        }

        public async Task<IEnumerable<Video>> GetRecentVideosAsync(int count)
        {
            return await _context.Videos
                .Include(v => v.User)
                .Where(v => v.Status == VideoStatus.Published)
                .OrderByDescending(v => v.CreatedAt)
                .Take(count)
                .ToListAsync();
        }
        public async Task<IEnumerable<Video>> GetByCategoryAsync(int categoryId, int count)
        {
            return await _context.Videos
                .Where(v => v.CategoryId == categoryId && v.Status == VideoStatus.Published)
                .Include(v => v.User)
                .OrderByDescending(v => v.CreatedAt)
                .Take(count)
                .ToListAsync();
        }
        public async Task<IEnumerable<Video>> GetRecommendedVideosAsync(int videoId, int count)
        {
            // Lấy video hiện tại để biết category
            var currentVideo = await _context.Videos
                .FirstOrDefaultAsync(v => v.Id == videoId);

            if (currentVideo == null)
                return Enumerable.Empty<Video>();

            // Lấy các video cùng category, trừ video hiện tại
            var sameCategory = await _context.Videos
                .Where(v => v.CategoryId == currentVideo.CategoryId &&
                            v.Id != videoId &&
                            v.Status == VideoStatus.Published)
                .Include(v => v.User)
                .OrderByDescending(v => v.ViewCount)
                .Take(count / 2)
                .ToListAsync();

            // Lấy thêm các video phổ biến khác nếu chưa đủ số lượng
            var remainingCount = count - sameCategory.Count;

            if (remainingCount > 0)
            {
                var otherVideos = await _context.Videos
                    .Where(v => v.Id != videoId &&
                                !sameCategory.Select(sc => sc.Id).Contains(v.Id) &&
                                v.Status == VideoStatus.Published)
                    .Include(v => v.User)
                    .OrderByDescending(v => v.ViewCount)
                    .Take(remainingCount)
                    .ToListAsync();

                return sameCategory.Concat(otherVideos);
            }

            return sameCategory;
        }


        public async Task<int> GetTotalCountAsync()
        {
            return await _context.Videos.CountAsync();
        }

        public async Task<IEnumerable<Video>> GetAllVideosForAdminAsync()
        {
            return await _context.Videos
                .Include(v => v.User)
                .Include(v => v.Category)
                .OrderByDescending(v => v.CreatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(Video video)
        {
            await _context.Videos.AddAsync(video);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Video video)
        {
            _context.Videos.Update(video);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var video = await _context.Videos.FindAsync(id);
            if (video != null)
            {
                video.Status = VideoStatus.Deleted;
                await _context.SaveChangesAsync();
            }
        }

        public async Task IncrementViewCountAsync(int id)
        {
            var video = await _context.Videos.FindAsync(id);
            if (video != null)
            {
                video.ViewCount++;
                await _context.SaveChangesAsync();
            }
        }

    }
}

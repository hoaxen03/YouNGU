using Microsoft.EntityFrameworkCore;
using YouNGU.Data;
using YouNGU.Models.Entities;

namespace YouNGU.Services
{
    public class TagService : ITagService
    {
        private readonly ApplicationDbContext _context;

        public TagService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Tag>> GetAllTagsAsync()
        {
            return await _context.Tags.ToListAsync();
        }

        public async Task<IEnumerable<Tag>> GetTagsByVideoIdAsync(int videoId)
        {
            return await _context.VideoTags
                .Where(vt => vt.VideoId == videoId)
                .Select(vt => vt.Tag)
                .ToListAsync();
        }

        public async Task AddTagsToVideoAsync(int videoId, string tagString)
        {
            if (string.IsNullOrWhiteSpace(tagString))
                return;

            // Xóa tags hiện tại của video
            await RemoveTagsFromVideoAsync(videoId);

            // Tách chuỗi tags
            var tagNames = tagString.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.Trim().ToLower())
                .Distinct()
                .ToList();

            foreach (var tagName in tagNames)
            {
                // Tìm tag trong database hoặc tạo mới
                var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name.ToLower() == tagName);
                if (tag == null)
                {
                    tag = new Tag { Name = tagName };
                    await _context.Tags.AddAsync(tag);
                    await _context.SaveChangesAsync();
                }

                // Thêm liên kết video-tag
                var videoTag = new VideoTag
                {
                    VideoId = videoId,
                    TagId = tag.Id
                };

                await _context.VideoTags.AddAsync(videoTag);
            }

            await _context.SaveChangesAsync();
        }

        public async Task RemoveTagsFromVideoAsync(int videoId)
        {
            var videoTags = await _context.VideoTags
                .Where(vt => vt.VideoId == videoId)
                .ToListAsync();

            _context.VideoTags.RemoveRange(videoTags);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Video>> GetVideosByTagAsync(string tagName)
        {
            return await _context.VideoTags
                .Where(vt => vt.Tag.Name.ToLower() == tagName.ToLower())
                .Select(vt => vt.Video)
                .Where(v => v.Status == VideoStatus.Published)
                .Include(v => v.User)
                .ToListAsync();
        }
        public async Task UpdateTagsForVideoAsync(int videoId, string tagString)
        {
            // Xóa tất cả các tag hiện tại của video
            var existingVideoTags = await _context.VideoTags
                .Where(vt => vt.VideoId == videoId)
                .ToListAsync();

            _context.VideoTags.RemoveRange(existingVideoTags);
            await _context.SaveChangesAsync();

            // Thêm các tag mới
            await AddTagsToVideoAsync(videoId, tagString);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using YouNGU.Models.Entities;

namespace YouNGU.Data.Repositories
{
    public class PlaylistRepository : IPlaylistRepository
    {
        private readonly ApplicationDbContext _context;

        public PlaylistRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Playlist>> GetAllByUserIdAsync(string userId)
        {
            return await _context.Playlists
                .Where(p => p.UserId == userId)
                .Include(p => p.PlaylistVideos)
                .ThenInclude(pv => pv.Video)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<Playlist> GetByIdAsync(int id)
        {
            return await _context.Playlists
                .Include(p => p.PlaylistVideos)
                .ThenInclude(pv => pv.Video)
                .ThenInclude(v => v.User)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Video>> GetPlaylistVideosAsync(int playlistId)
        {
            return await _context.PlaylistVideos
                .Where(pv => pv.PlaylistId == playlistId)
                .OrderBy(pv => pv.Order)
                .Select(pv => pv.Video)
                .Include(v => v.User)
                .ToListAsync();
        }

        public async Task AddAsync(Playlist playlist)
        {
            await _context.Playlists.AddAsync(playlist);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Playlist playlist)
        {
            _context.Playlists.Update(playlist);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var playlist = await _context.Playlists.FindAsync(id);
            if (playlist != null)
            {
                _context.Playlists.Remove(playlist);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddVideoToPlaylistAsync(int playlistId, int videoId, int order = 0)
        {
            // Kiểm tra xem video đã có trong playlist chưa
            var existingItem = await _context.PlaylistVideos
                .FirstOrDefaultAsync(pv => pv.PlaylistId == playlistId && pv.VideoId == videoId);

            if (existingItem == null)
            {
                // Nếu không có order được chỉ định, thêm vào cuối
                if (order == 0)
                {
                    var maxOrder = await _context.PlaylistVideos
                        .Where(pv => pv.PlaylistId == playlistId)
                        .Select(pv => (int?)pv.Order)
                        .MaxAsync() ?? 0;

                    order = maxOrder + 1;
                }

                var playlistVideo = new PlaylistVideo
                {
                    PlaylistId = playlistId,
                    VideoId = videoId,
                    Order = order
                };

                await _context.PlaylistVideos.AddAsync(playlistVideo);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveVideoFromPlaylistAsync(int playlistId, int videoId)
        {
            var playlistVideo = await _context.PlaylistVideos
                .FirstOrDefaultAsync(pv => pv.PlaylistId == playlistId && pv.VideoId == videoId);

            if (playlistVideo != null)
            {
                _context.PlaylistVideos.Remove(playlistVideo);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ReorderPlaylistVideosAsync(int playlistId, Dictionary<int, int> videoOrders)
        {
            foreach (var item in videoOrders)
            {
                var videoId = item.Key;
                var newOrder = item.Value;

                var playlistVideo = await _context.PlaylistVideos
                    .FirstOrDefaultAsync(pv => pv.PlaylistId == playlistId && pv.VideoId == videoId);

                if (playlistVideo != null)
                {
                    playlistVideo.Order = newOrder;
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}

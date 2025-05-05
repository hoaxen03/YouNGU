using YouNGU.Models.Entities;

namespace YouNGU.Data.Repositories
{
    public interface IPlaylistRepository
    {
        Task<IEnumerable<Playlist>> GetAllByUserIdAsync(string userId);
        Task<Playlist> GetByIdAsync(int id);
        Task<IEnumerable<Video>> GetPlaylistVideosAsync(int playlistId);
        Task AddAsync(Playlist playlist);
        Task UpdateAsync(Playlist playlist);
        Task DeleteAsync(int id);
        Task AddVideoToPlaylistAsync(int playlistId, int videoId, int order = 0);
        Task RemoveVideoFromPlaylistAsync(int playlistId, int videoId);
        Task ReorderPlaylistVideosAsync(int playlistId, Dictionary<int, int> videoOrders);

    }
}

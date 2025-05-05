using YouNGU.Models.Entities;

namespace YouNGU.Data.Repositories
{
    public interface IVideoRepository
    {
        Task<IEnumerable<Video>> GetAllAsync();
        Task<Video> GetByIdAsync(int id);
        Task<IEnumerable<Video>> GetByUserIdAsync(string userId);
        Task<IEnumerable<Video>> SearchAsync(string searchTerm);
        Task<IEnumerable<Video>> GetRecentVideosAsync(int count);
        Task<IEnumerable<Video>> GetRecommendedVideosAsync(int videoId, int count);
        Task<IEnumerable<Video>> GetByCategoryAsync(int categoryId, int count);
        Task<int> GetTotalCountAsync();
        Task<IEnumerable<Video>> GetAllVideosForAdminAsync();
        Task AddAsync(Video video);
        Task UpdateAsync(Video video);
        Task DeleteAsync(int id);
        Task IncrementViewCountAsync(int id);
    }
}

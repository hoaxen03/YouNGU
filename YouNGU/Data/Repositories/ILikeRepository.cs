using YouNGU.Models.Entities;

namespace YouNGU.Data.Repositories
{
    public interface ILikeRepository
    {
        Task<IEnumerable<Like>> GetByVideoIdAsync(int videoId);
        Task<bool> HasUserLikedVideoAsync(string userId, int videoId);
        Task ToggleLikeAsync(int videoId, string userId);
        Task<int> GetLikeCountAsync(int videoId);

    }
}

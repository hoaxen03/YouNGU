using YouNGU.Models.Entities;

namespace YouNGU.Services
{
    public interface ITagService
    {
        Task<IEnumerable<Tag>> GetAllTagsAsync();
        Task<IEnumerable<Tag>> GetTagsByVideoIdAsync(int videoId);
        Task AddTagsToVideoAsync(int videoId, string tagString);
        Task RemoveTagsFromVideoAsync(int videoId);
        Task<IEnumerable<Video>> GetVideosByTagAsync(string tagName);
        Task UpdateTagsForVideoAsync(int videoId, string tagString);

    }
}

using YouNGU.Models.Entities;

namespace YouNGU.Data.Repositories
{
    public interface ICommentRepository
    {
        Task<IEnumerable<Comment>> GetByVideoIdAsync(int videoId);
        Task<IEnumerable<Comment>> GetAllAsync();
        Task<IEnumerable<Comment>> GetAllCommentsForAdminAsync();
        Task<Comment> GetByIdAsync(int id);
        Task<Comment> AddAsync(Comment comment);
        Task<Comment> UpdateAsync(Comment comment);
        Task DeleteAsync(int id);
        Task<IEnumerable<Comment>> GetCommentsByVideoIdAsync(int videoId);
        Task<IEnumerable<Comment>> GetCommentsByUserIdAsync(string userId);
    }
}

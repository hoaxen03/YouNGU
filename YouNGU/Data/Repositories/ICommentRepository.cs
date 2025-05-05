using YouNGU.Models.Entities;

namespace YouNGU.Data.Repositories
{
    public interface ICommentRepository
    {
        Task<IEnumerable<Comment>> GetByVideoIdAsync(int videoId);
        Task AddAsync(Comment comment);
        Task UpdateAsync(Comment comment);
        Task DeleteAsync(int id);
    }
}

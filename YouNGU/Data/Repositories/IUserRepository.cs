using YouNGU.Models.Entities;

namespace YouNGU.Data.Repositories
{
    public interface IUserRepository
    {
        Task<ApplicationUser> GetByIdAsync(string id);
        Task<IEnumerable<ApplicationUser>> GetAllAsync();
        Task<IEnumerable<ApplicationUser>> GetRecentUsersAsync(int count);
        Task<int> GetTotalCountAsync();
        Task UpdateAsync(ApplicationUser user);

    }
}

using Microsoft.EntityFrameworkCore;
using YouNGU.Models.Entities;

namespace YouNGU.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApplicationUser> GetByIdAsync(string id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<IEnumerable<ApplicationUser>> GetRecentUsersAsync(int count)
        {
            return await _context.Users
                .OrderByDescending(u => u.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _context.Users.CountAsync();
        }

        public async Task UpdateAsync(ApplicationUser user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}

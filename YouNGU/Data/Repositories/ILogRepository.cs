using YouNGU.Models;

namespace YouNGU.Repositories
{
    public interface ILogRepository
    {
        Task<IEnumerable<Log>> GetLogsAsync(
            string level = null,
            string category = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string searchTerm = null,
            int page = 1,
            int pageSize = 50);

        Task<Log> GetLogByIdAsync(int id);

        Task<int> GetLogsCountAsync(
            string level = null,
            string category = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string searchTerm = null);

        Task DeleteLogAsync(int id);

        Task ClearLogsAsync(string category = null);

        Task<IEnumerable<string>> GetLogCategoriesAsync();

        Task<IEnumerable<string>> GetLogLevelsAsync();
    }
}

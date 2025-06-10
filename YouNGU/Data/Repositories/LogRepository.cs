using System;
using Microsoft.EntityFrameworkCore;
using YouNGU.Data;
using YouNGU.Models;

namespace YouNGU.Repositories
{
    public class LogRepository : ILogRepository
    {
        private readonly ApplicationDbContext _context;

        public LogRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Log>> GetLogsAsync(
            string level = null,
            string category = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string searchTerm = null,
            int page = 1,
            int pageSize = 50)
        {
            IQueryable<Log> query = _context.Logs;

            // Áp dụng các bộ lọc
            if (!string.IsNullOrEmpty(level))
            {
                query = query.Where(l => l.Level == level);
            }

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(l => l.SourceContext.Contains(category));
            }

            if (fromDate.HasValue)
            {
                query = query.Where(l => l.Timestamp >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(l => l.Timestamp <= toDate.Value.AddDays(1).AddSeconds(-1));
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(l =>
                    l.Message.Contains(searchTerm) ||
                    l.Exception.Contains(searchTerm) ||
                    (l.AdditionalInfo != null && l.AdditionalInfo.Contains(searchTerm)) ||
                    (l.RequestPath != null && l.RequestPath.Contains(searchTerm)) ||
                    (l.SourceContext != null && l.SourceContext.Contains(searchTerm)) ||
                    (l.ActionName != null && l.ActionName.Contains(searchTerm)));
            }
            // Sắp xếp và phân trang
            return await query
                .OrderByDescending(l => l.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Log> GetLogByIdAsync(int id)
        {
            return await _context.Logs.FindAsync(id);
        }

        public async Task<int> GetLogsCountAsync(
            string level = null,
            string category = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string searchTerm = null)
        {
            IQueryable<Log> query = _context.Logs;

            // Áp dụng các bộ lọc
            if (!string.IsNullOrEmpty(level))
            {
                query = query.Where(l => l.Level == level);
            }

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(l => l.Category == category);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(l => l.Timestamp >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(l => l.Timestamp <= toDate.Value.AddDays(1).AddSeconds(-1));
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(l =>
                    l.Message.Contains(searchTerm) ||
                    l.Exception.Contains(searchTerm) ||
                    l.AdditionalInfo.Contains(searchTerm) ||
                    l.RequestPath.Contains(searchTerm) ||
                    l.SourceContext.Contains(searchTerm) ||
                    l.ActionName.Contains(searchTerm));
            }

            return await query.CountAsync();
        }

        public async Task DeleteLogAsync(int id)
        {
            var log = await _context.Logs.FindAsync(id);
            if (log != null)
            {
                _context.Logs.Remove(log);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ClearLogsAsync(string category = null)
        {
            if (string.IsNullOrEmpty(category))
            {
                // Xóa tất cả log
                _context.Logs.RemoveRange(_context.Logs);
            }
            else
            {
                // Xóa log theo category
                var logs = await _context.Logs.Where(l => l.Category == category).ToListAsync();
                _context.Logs.RemoveRange(logs);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<string>> GetLogCategoriesAsync()
        {
            return await _context.Logs
                .Select(l => l.Category)
                .Distinct()
                .Where(c => c != null)
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> GetLogLevelsAsync()
        {
            return await _context.Logs
                .Select(l => l.Level)
                .Distinct()
                .Where(l => l != null)
                .ToListAsync();
        }
    }
}

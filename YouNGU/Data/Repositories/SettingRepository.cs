using Microsoft.EntityFrameworkCore;
using YouNGU.Models.Entities;

namespace YouNGU.Data.Repositories
{
    public class SettingRepository : ISettingRepository
    {
        private readonly ApplicationDbContext _context;

        public SettingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Setting> GetByKeyAsync(string key)
        {
            return await _context.Settings.FirstOrDefaultAsync(s => s.Key == key);
        }

        public async Task<string> GetValueAsync(string key, string defaultValue = null)
        {
            var setting = await GetByKeyAsync(key);
            return setting?.Value ?? defaultValue;
        }

        public async Task<int> GetIntValueAsync(string key, int defaultValue = 0)
        {
            var value = await GetValueAsync(key);
            return int.TryParse(value, out int result) ? result : defaultValue;
        }

        public async Task<bool> GetBoolValueAsync(string key, bool defaultValue = false)
        {
            var value = await GetValueAsync(key);
            return bool.TryParse(value, out bool result) ? result : defaultValue;
        }

        public async Task<IEnumerable<Setting>> GetAllAsync()
        {
            return await _context.Settings.ToListAsync();
        }

        public async Task SaveSettingAsync(string key, string value, string description = null, string type = "string")
        {
            var setting = await GetByKeyAsync(key);

            if (setting == null)
            {
                setting = new Setting
                {
                    Key = key,
                    Value = value,
                    Description = description,
                    Type = type,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.Settings.Add(setting);
            }
            else
            {
                setting.Value = value;
                setting.UpdatedAt = DateTime.UtcNow;

                if (!string.IsNullOrEmpty(description))
                {
                    setting.Description = description;
                }

                if (!string.IsNullOrEmpty(type))
                {
                    setting.Type = type;
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task SaveSettingsAsync(Dictionary<string, string> settings)
        {
            foreach (var kvp in settings)
            {
                var setting = await GetByKeyAsync(kvp.Key);

                if (setting == null)
                {
                    setting = new Setting
                    {
                        Key = kvp.Key,
                        Value = kvp.Value,
                        UpdatedAt = DateTime.UtcNow
                    };
                    _context.Settings.Add(setting);
                }
                else
                {
                    setting.Value = kvp.Value;
                    setting.UpdatedAt = DateTime.UtcNow;
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}

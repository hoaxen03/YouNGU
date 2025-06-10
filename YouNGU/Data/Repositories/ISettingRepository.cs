using YouNGU.Models.Entities;

namespace YouNGU.Data.Repositories
{
    public interface ISettingRepository
    {
        Task<Setting> GetByKeyAsync(string key);
        Task<string> GetValueAsync(string key, string defaultValue = null);
        Task<int> GetIntValueAsync(string key, int defaultValue = 0);
        Task<bool> GetBoolValueAsync(string key, bool defaultValue = false);
        Task<IEnumerable<Setting>> GetAllAsync();
        Task SaveSettingAsync(string key, string value, string description = null, string type = "string");
        Task SaveSettingsAsync(Dictionary<string, string> settings);
    }
}

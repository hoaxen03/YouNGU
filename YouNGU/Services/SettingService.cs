using YouNGU.Data.Repositories;

namespace YouNGU.Services
{
    public class SettingService
    {
        private readonly ISettingRepository _settingRepository;

        public SettingService(ISettingRepository settingRepository)
        {
            _settingRepository = settingRepository;
        }

        public async Task<string> GetSiteNameAsync()
        {
            return await _settingRepository.GetValueAsync("SiteSettings:SiteName", "YouNGU");
        }

        public async Task<string> GetSiteDescriptionAsync()
        {
            return await _settingRepository.GetValueAsync("SiteSettings:SiteDescription", "Nền tảng chia sẻ video");
        }

        public async Task<string> GetContactEmailAsync()
        {
            return await _settingRepository.GetValueAsync("SiteSettings:ContactEmail", "contact@youngu.com");
        }

        public async Task<int> GetMaxUploadSizeMBAsync()
        {
            return await _settingRepository.GetIntValueAsync("SiteSettings:MaxUploadSizeMB", 500);
        }

        public async Task<string> GetAllowedVideoExtensionsAsync()
        {
            return await _settingRepository.GetValueAsync("SiteSettings:AllowedVideoExtensions", ".mp4,.mov,.avi,.wmv");
        }

        public async Task<bool> IsCommentEnabledAsync()
        {
            return await _settingRepository.GetBoolValueAsync("SiteSettings:EnableComments", true);
        }

        public async Task<bool> IsCommentApprovalRequiredAsync()
        {
            return await _settingRepository.GetBoolValueAsync("SiteSettings:RequireCommentApproval", true);
        }

        public async Task<bool> IsUserRegistrationEnabledAsync()
        {
            return await _settingRepository.GetBoolValueAsync("SiteSettings:EnableUserRegistration", true);
        }

        public async Task<string> GetDefaultUserRoleAsync()
        {
            return await _settingRepository.GetValueAsync("SiteSettings:DefaultUserRole", "User");
        }
    }
}

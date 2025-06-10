using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using YouNGU.Areas.Admin.Models;
using YouNGU.Data.Repositories;

namespace YouNGU.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [Route("Admin/[controller]")]
    public class SettingsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ISettingRepository _settingRepository;

        public SettingsController(
            IConfiguration configuration,
            ISettingRepository settingRepository)
        {
            _configuration = configuration;
            _settingRepository = settingRepository;
        }

        [HttpGet]
        [Route("")]
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            var model = new SettingsViewModel
            {
                SiteName = await _settingRepository.GetValueAsync("SiteSettings:SiteName", "YouNGU"),
                SiteDescription = await _settingRepository.GetValueAsync("SiteSettings:SiteDescription", "Nền tảng chia sẻ video"),
                ContactEmail = await _settingRepository.GetValueAsync("SiteSettings:ContactEmail", "contact@youngu.com"),
                MaxUploadSizeMB = await _settingRepository.GetIntValueAsync("SiteSettings:MaxUploadSizeMB", 500),
                AllowedVideoExtensions = await _settingRepository.GetValueAsync("SiteSettings:AllowedVideoExtensions", ".mp4,.mov,.avi,.wmv"),
                EnableComments = await _settingRepository.GetBoolValueAsync("SiteSettings:EnableComments", true),
                RequireCommentApproval = await _settingRepository.GetBoolValueAsync("SiteSettings:RequireCommentApproval", true),
                EnableUserRegistration = await _settingRepository.GetBoolValueAsync("SiteSettings:EnableUserRegistration", true),
                DefaultUserRole = await _settingRepository.GetValueAsync("SiteSettings:DefaultUserRole", "User")
            };

            return View(model);
        }

        [HttpPost]
        [Route("")]
        [Route("Index")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(SettingsViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Tạo dictionary chứa tất cả các cài đặt cần lưu
                var settings = new Dictionary<string, string>
                {
                    { "SiteSettings:SiteName", model.SiteName },
                    { "SiteSettings:SiteDescription", model.SiteDescription ?? "" },
                    { "SiteSettings:ContactEmail", model.ContactEmail },
                    { "SiteSettings:MaxUploadSizeMB", model.MaxUploadSizeMB.ToString() },
                    { "SiteSettings:AllowedVideoExtensions", model.AllowedVideoExtensions },
                    { "SiteSettings:EnableComments", model.EnableComments.ToString() },
                    { "SiteSettings:RequireCommentApproval", model.RequireCommentApproval.ToString() },
                    { "SiteSettings:EnableUserRegistration", model.EnableUserRegistration.ToString() },
                    { "SiteSettings:DefaultUserRole", model.DefaultUserRole }
                };

                // Lưu tất cả các cài đặt vào cơ sở dữ liệu
                await _settingRepository.SaveSettingsAsync(settings);

                TempData["SuccessMessage"] = "Cài đặt đã được lưu thành công.";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [HttpGet]
        [Route("Reset")]
        public async Task<IActionResult> Reset()
        {
            // Đặt lại các cài đặt về giá trị mặc định
            var defaultSettings = new Dictionary<string, string>
            {
                { "SiteSettings:SiteName", "YouNGU" },
                { "SiteSettings:SiteDescription", "Nền tảng chia sẻ video" },
                { "SiteSettings:ContactEmail", "contact@youngu.com" },
                { "SiteSettings:MaxUploadSizeMB", "500" },
                { "SiteSettings:AllowedVideoExtensions", ".mp4,.mov,.avi,.wmv" },
                { "SiteSettings:EnableComments", "true" },
                { "SiteSettings:RequireCommentApproval", "true" },
                { "SiteSettings:EnableUserRegistration", "true" },
                { "SiteSettings:DefaultUserRole", "User" }
            };

            await _settingRepository.SaveSettingsAsync(defaultSettings);

            TempData["SuccessMessage"] = "Cài đặt đã được đặt lại về giá trị mặc định.";
            return RedirectToAction(nameof(Index));
        }
    }
}

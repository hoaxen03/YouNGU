using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using YouNGU.Areas.Admin.Models;
using YouNGU.Data.Repositories;

namespace YouNGU.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Route("Admin/[controller]")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly IVideoRepository _videoRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<DashboardController> _logger;
        private readonly ILogService _logService;

        public DashboardController(
            IVideoRepository videoRepository,
            IUserRepository userRepository,
            ILogger<DashboardController> logger,
            ILogService logService)
        {
            _videoRepository = videoRepository;
            _userRepository = userRepository;
            _logger = logger;
            _logService = logService;
        }

        public async Task<IActionResult> Index(string userId)
        {
            try
            {
                _logger.LogInformation("DashboardController: Index action started.");
                _logService.LogUserActivity(userId, "Đăng nhập thành công");
                // Get total counts
                var totalVideos = await _videoRepository.GetTotalCountAsync();
                var totalUsers = await _userRepository.GetTotalCountAsync();

                // Get recent videos and users
                var recentVideos = await _videoRepository.GetAllVideosForAdminAsync();
                var recentUsers = await _userRepository.GetRecentUsersAsync(10);

                // Calculate video status distribution
                var allVideos = await _videoRepository.GetAllVideosForAdminAsync();
                var videoStatusCounts = allVideos
                    .GroupBy(v => v.Status)
                    .ToDictionary(g => g.Key, g => g.Count());

                // Create the view model
                var model = new AdminDashboardViewModel
                {
                    TotalVideos = totalVideos,
                    TotalUsers = totalUsers,
                    RecentVideos = recentVideos.Take(10).ToList(),
                    RecentUsers = recentUsers.ToList(),
                    VideoStatusCounts = videoStatusCounts
                };
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DashboardController: An error occurred while loading the dashboard.");
                return StatusCode(500, "An error occurred while loading the dashboard.");
            }
        }

    }
}

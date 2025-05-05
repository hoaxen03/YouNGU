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

        public DashboardController(
            IVideoRepository videoRepository,
            IUserRepository userRepository,
            ILogger<DashboardController> logger)
        {
            _videoRepository = videoRepository;
            _userRepository = userRepository;
            _logger = logger;

        }

        public async Task<IActionResult> Index()
        {
            try
            {
                _logger.LogInformation("DashboardController: Index action started.");
                        var recentVideos = await _videoRepository.GetRecentVideosAsync(10);
                var model = new AdminDashboardViewModel
                {
                    TotalVideos = await _videoRepository.GetTotalCountAsync(),
                    TotalUsers = await _userRepository.GetTotalCountAsync(),
                    RecentVideos = recentVideos.Select(video => new RecentVideoViewModel
                    {
                        Id = video.Id,
                        Title = video.Title,
                        Description = video.Description,
                        CreatedAt = video.CreatedAt,
                        ThumbnailUrl = video.ThumbnailUrl,
                        UploaderName = video.User?.UserName ?? "Unknown",
                        Status = video.Status.ToString(),
                        ViewCount = video.ViewCount,
                        VideoUrl = video.CloudinaryUrl,
                        UserId = video.UserId
                    }).ToList(),
                    RecentUsers = await _userRepository.GetRecentUsersAsync(10)
                };

                _logger.LogInformation("DashboardController: Data successfully retrieved for the dashboard.");
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

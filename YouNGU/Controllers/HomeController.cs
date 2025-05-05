using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using YouNGU.Data.Repositories;
using YouNGU.Models;
using YouNGU.Models.ViewModels;

namespace YouNGU.Controllers
{
    public class HomeController : Controller
    {
        private readonly IVideoRepository _videoRepository;
        private readonly ICategoryRepository _categoryRepository;

        public HomeController(
            IVideoRepository videoRepository,
            ICategoryRepository categoryRepository)
        {
            _videoRepository = videoRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<IActionResult> Index()
        {
            var recentVideos = await _videoRepository.GetRecentVideosAsync(12);
            var popularVideos = await _videoRepository.GetAllAsync(); // Thực tế sẽ lọc theo lượt xem
            var categories = await _categoryRepository.GetAllAsync();

            var model = new HomeViewModel
            {
                RecentVideos = recentVideos,
                PopularVideos = popularVideos,
                Categories = categories
            };

            return View(model);
        }

        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return RedirectToAction(nameof(Index));
            }

            var videos = await _videoRepository.SearchAsync(query);
            ViewData["SearchQuery"] = query;
            return View(videos);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

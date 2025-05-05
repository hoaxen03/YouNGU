using Microsoft.AspNetCore.Mvc;
using YouNGU.Services; // Giả sử bạn có một service để truy vấn dữ liệu
using YouNGU.Models.Entities;
using YouNGU.Data.Repositories;

namespace YouNGU.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IVideoRepository _videoService;

        public CategoryController(IVideoRepository videoService)
        {
            _videoService = videoService;
        }

        public async Task<IActionResult> Videos(int id)
        {
            // Lấy danh sách video theo idCategory
            var videos = await _videoService.GetByCategoryAsync(id, 10); // Giới hạn 10 video

            if (videos == null || !videos.Any())
            {
                return View(new List<Video>()); // Trả về view với danh sách rỗng
            }

            return View(videos); // Trả về view với danh sách video
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using YouNGU.Areas.Admin.Models;
using YouNGU.Data.Repositories;
using YouNGU.Models.Entities;
using YouNGU.Services;

namespace YouNGU.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [Route("Admin/[controller]")]
    public class VideoManagementController : Controller
    {
        private readonly IVideoRepository _videoRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly CloudinaryService _cloudinaryService;
        private readonly ILogger<VideoManagementController> _logger;

        public VideoManagementController(
            IVideoRepository videoRepository,
            ICategoryRepository categoryRepository,
            CloudinaryService cloudinaryService,
            ILogger<VideoManagementController> logger)
        {
            _videoRepository = videoRepository;
            _categoryRepository = categoryRepository;
            _cloudinaryService = cloudinaryService;
            _logger = logger;
        }

        [HttpGet]
        [Route("")]
        [Route("Index")]
        public async Task<IActionResult> Index(string searchTerm = "", string status = "")
        {
            var videos = await _videoRepository.GetAllVideosForAdminAsync();

            // Lọc theo từ khóa tìm kiếm
            if (!string.IsNullOrEmpty(searchTerm))
            {
                videos = videos.Where(v =>
                    v.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    v.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (v.User != null && v.User.UserName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                );
            }

            // Lọc theo trạng thái
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<VideoStatus>(status, out var videoStatus))
            {
                videos = videos.Where(v => v.Status == videoStatus);
            }

            return View(videos);
        }

        [HttpGet]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var video = await _videoRepository.GetByIdAsync(id);
            if (video == null)
            {
                return NotFound();
            }

            var categories = await _categoryRepository.GetAllAsync();
            var model = new EditVideoViewModel
            {
                Id = video.Id,
                Title = video.Title,
                Description = video.Description,
                CategoryId = video.CategoryId,
                Status = video.Status,
                Categories = new SelectList(categories, "Id", "Name"),
                Video = video
            };

            return View(model);
        }

        [HttpPost]
        [Route("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditVideoViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var video = await _videoRepository.GetByIdAsync(id);
                    if (video == null)
                    {
                        return NotFound();
                    }

                    video.Title = model.Title;
                    video.Description = model.Description;
                    video.CategoryId = model.CategoryId;
                    video.Status = model.Status;
                    video.UpdatedAt = DateTime.UtcNow;

                    await _videoRepository.UpdateAsync(video);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Đã xảy ra lỗi khi cập nhật video.");
                }
            }

            // Nếu có lỗi, lấy lại danh sách danh mục và trả về view
            var categories = await _categoryRepository.GetAllAsync();
            model.Categories = new SelectList(categories, "Id", "Name");
            model.Video = await _videoRepository.GetByIdAsync(id);

            return View(model);
        }

        [HttpGet]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var video = await _videoRepository.GetByIdAsync(id);
            if (video == null)
            {
                return NotFound();
            }
            return View(video);
        }

        [HttpPost]
        [Route("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var video = await _videoRepository.GetByIdAsync(id);
                if (video == null)
                {
                    return NotFound();
                }

                // Xóa video từ Cloudinary nếu có publicId
                if (!string.IsNullOrEmpty(video.CloudinaryPublicId))
                {
                    await _cloudinaryService.DeleteVideoAsync(video.CloudinaryPublicId);
                }

                // Xóa video từ cơ sở dữ liệu
                await _videoRepository.DeleteAsync(id);

                _logger.LogInformation("Video {VideoId} đã bị xóa bởi {UserId}", id, User.Identity.Name);
                TempData["SuccessMessage"] = "Video đã được xóa thành công.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa video {VideoId}", id);
                TempData["ErrorMessage"] = $"Lỗi khi xóa video: {ex.Message}";
                return RedirectToAction(nameof(Delete), new { id });
            }
        }
        [HttpPost]
        [Route("QuickApprove/{id}")]
        public async Task<IActionResult> QuickApprove(int id)
        {
            try
            {
                var video = await _videoRepository.GetByIdAsync(id);
                if (video == null)
                {
                    return Json(new { success = false, message = "Video không tìm thấy" });
                }

                video.Status = VideoStatus.Public;
                video.UpdatedAt = DateTime.UtcNow;

                await _videoRepository.UpdateAsync(video);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
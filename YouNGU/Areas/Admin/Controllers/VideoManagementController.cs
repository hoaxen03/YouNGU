using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using YouNGU.Areas.Admin.Models;
using YouNGU.Data.Repositories;

namespace YouNGU.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class VideoManagementController : Controller
    {
        private readonly IVideoRepository _videoRepository;
        private readonly ICategoryRepository _categoryRepository;

        public VideoManagementController(
            IVideoRepository videoRepository,
            ICategoryRepository categoryRepository)
        {
            _videoRepository = videoRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<IActionResult> Index()
        {
            var videos = await _videoRepository.GetAllVideosForAdminAsync();
            return View(videos);
        }

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
                Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name,
                    Selected = c.Id == video.CategoryId
                })
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditVideoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var categories = await _categoryRepository.GetAllAsync();
                model.Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name,
                    Selected = c.Id == model.CategoryId
                });
                return View(model);
            }

            var video = await _videoRepository.GetByIdAsync(model.Id);
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

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _videoRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}

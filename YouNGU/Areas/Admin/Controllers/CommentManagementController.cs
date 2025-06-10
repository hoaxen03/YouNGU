using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YouNGU.Areas.Admin.Models;
using YouNGU.Data.Repositories;
using YouNGU.Models.Entities;

namespace YouNGU.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [Route("Admin/[controller]")]
    public class CommentManagementController : Controller
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IVideoRepository _videoRepository;

        public CommentManagementController(
            ICommentRepository commentRepository,
            IVideoRepository videoRepository)
        {
            _commentRepository = commentRepository;
            _videoRepository = videoRepository;
        }

        [HttpGet]
        [Route("")]
        [Route("Index")]
        public async Task<IActionResult> Index(string searchTerm = "", string status = "")
        {
            var comments = await _commentRepository.GetAllCommentsForAdminAsync();

            // Lọc theo từ khóa tìm kiếm
            if (!string.IsNullOrEmpty(searchTerm))
            {
                comments = comments.Where(c =>
                    c.Content.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (c.User != null && c.User.UserName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (c.Video != null && c.Video.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                );
            }

            // Lọc theo trạng thái
            if (!string.IsNullOrEmpty(status))
            {
                bool isApproved = status == "approved";
                comments = comments.Where(c => c.IsApproved == isApproved);
            }

            return View(comments);
        }

        [HttpGet]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var comment = await _commentRepository.GetByIdAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            var model = new EditCommentViewModel
            {
                Id = comment.Id,
                Content = comment.Content,
                IsApproved = comment.IsApproved,
                Comment = comment
            };

            return View(model);
        }

        [HttpPost]
        [Route("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditCommentViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var comment = await _commentRepository.GetByIdAsync(id);
                if (comment == null)
                {
                    return NotFound();
                }

                comment.Content = model.Content;
                comment.IsApproved = model.IsApproved;
                comment.UpdatedAt = DateTime.UtcNow;

                await _commentRepository.UpdateAsync(comment);
                return RedirectToAction(nameof(Index));
            }

            model.Comment = await _commentRepository.GetByIdAsync(id);
            return View(model);
        }

        [HttpPost]
        [Route("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _commentRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Route("QuickApprove/{id}")]
        public async Task<IActionResult> QuickApprove(int id)
        {
            try
            {
                var comment = await _commentRepository.GetByIdAsync(id);
                if (comment == null)
                {
                    return Json(new { success = false, message = "Bình luận không tìm thấy" });
                }

                comment.IsApproved = true;
                comment.UpdatedAt = DateTime.UtcNow;

                await _commentRepository.UpdateAsync(comment);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Route("QuickReject/{id}")]
        public async Task<IActionResult> QuickReject(int id)
        {
            try
            {
                var comment = await _commentRepository.GetByIdAsync(id);
                if (comment == null)
                {
                    return Json(new { success = false, message = "Bình luận không tìm thấy" });
                }

                comment.IsApproved = false;
                comment.UpdatedAt = DateTime.UtcNow;

                await _commentRepository.UpdateAsync(comment);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using YouNGU.Data.Repositories;
using YouNGU.Models.Entities;
using YouNGU.Models.ViewModels;

namespace YouNGU.Controllers
{
    public class PlaylistController : Controller
    {
        private readonly IPlaylistRepository _playlistRepository;
        private readonly IVideoRepository _videoRepository;

        public PlaylistController(
            IPlaylistRepository playlistRepository,
            IVideoRepository videoRepository)
        {
            _playlistRepository = playlistRepository;
            _videoRepository = videoRepository;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var playlists = await _playlistRepository.GetAllByUserIdAsync(userId);
            return View(playlists);
        }

        public async Task<IActionResult> View(int id)
        {
            var playlist = await _playlistRepository.GetByIdAsync(id);
            if (playlist == null)
            {
                return NotFound();
            }

            // Kiểm tra quyền truy cập
            if (!playlist.IsPublic)
            {
                if (!User.Identity.IsAuthenticated ||
                    User.FindFirstValue(ClaimTypes.NameIdentifier) != playlist.UserId)
                {
                    return Forbid();
                }
            }

            var videos = await _playlistRepository.GetPlaylistVideosAsync(id);
            bool isOwner = User.Identity.IsAuthenticated &&
                           User.FindFirstValue(ClaimTypes.NameIdentifier) == playlist.UserId;

            var model = new PlaylistViewModel
            {
                Playlist = playlist,
                Videos = videos,
                IsOwner = isOwner
            };

            return View(model);
        }

        [Authorize]
        public IActionResult Create()
        {
            return View(new CreatePlaylistViewModel());
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(CreatePlaylistViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var playlist = new Playlist
            {
                Name = model.Name,
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                IsPublic = model.IsPublic,
                CreatedAt = DateTime.UtcNow
            };

            await _playlistRepository.AddAsync(playlist);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddVideo(int playlistId, int videoId)
        {
            var playlist = await _playlistRepository.GetByIdAsync(playlistId);
            if (playlist == null)
            {
                return NotFound();
            }

            // Kiểm tra quyền
            if (User.FindFirstValue(ClaimTypes.NameIdentifier) != playlist.UserId)
            {
                return Forbid();
            }

            await _playlistRepository.AddVideoToPlaylistAsync(playlistId, videoId);
            return Json(new { success = true });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RemoveVideo(int playlistId, int videoId)
        {
            var playlist = await _playlistRepository.GetByIdAsync(playlistId);
            if (playlist == null)
            {
                return NotFound();
            }

            // Kiểm tra quyền
            if (User.FindFirstValue(ClaimTypes.NameIdentifier) != playlist.UserId)
            {
                return Forbid();
            }

            await _playlistRepository.RemoveVideoFromPlaylistAsync(playlistId, videoId);
            return Json(new { success = true });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var playlist = await _playlistRepository.GetByIdAsync(id);
            if (playlist == null)
            {
                return NotFound();
            }

            // Kiểm tra quyền
            if (User.FindFirstValue(ClaimTypes.NameIdentifier) != playlist.UserId)
            {
                return Forbid();
            }

            await _playlistRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}

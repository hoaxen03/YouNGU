using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using YouNGU.Data.Repositories;
using YouNGU.Models.Entities;
using YouNGU.Models.ViewModels;
using YouNGU.Models;
using YouNGU.Services;
using EditVideoViewModel = YouNGU.Models.ViewModels.EditVideoViewModel;
using Azure.Core;

namespace YouNGU.Controllers
{
    public class VideoController : Controller
    {
        private readonly IVideoRepository _videoRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IVideoService _videoService;
        private readonly ICommentRepository _commentRepository;
        private readonly ILikeRepository _likeRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IConfiguration _configuration;
        private readonly CloudinaryService _cloudinaryService;
        private readonly ILogger<VideoController> _logger; // Thêm ILogger
        private readonly ITagService _tagService; // Inject ITagService trực tiếp nếu có thể


        public VideoController(
            IVideoRepository videoRepository,
            ICategoryRepository categoryRepository,
            IVideoService videoService,
            ICommentRepository commentRepository,
            ILikeRepository likeRepository,
            ISubscriptionRepository subscriptionRepository,
            IConfiguration configuration,
            CloudinaryService cloudinaryService,
            ILogger<VideoController> logger, // Inject ILogger
            ITagService tagService // Inject ITagService
            )
        {
            _videoRepository = videoRepository;
            _categoryRepository = categoryRepository;
            _videoService = videoService;
            _commentRepository = commentRepository;
            _likeRepository = likeRepository;
            _subscriptionRepository = subscriptionRepository;
            _configuration = configuration;
            _cloudinaryService = cloudinaryService;
            _logger = logger; // Gán logger
            _tagService = tagService; // Gán tagService
        }

        [HttpGet]
        public async Task<IActionResult> Watch(int id)
        {
            var video = await _videoRepository.GetByIdAsync(id);
            if (video == null)
            {
                return NotFound();
            }

            // Kiểm tra quyền truy cập
            if (video.Status != VideoStatus.Published)
            {
                if (!User.Identity.IsAuthenticated || User.FindFirstValue(ClaimTypes.NameIdentifier) != video.UserId)
                {
                    return Forbid();
                }
            }

            // Tăng số lượt xem
            await _videoRepository.IncrementViewCountAsync(id);

            // Lấy video đề xuất
            var recommendedVideos = await _videoRepository.GetRecommendedVideosAsync(id, 10);

            // Kiểm tra người dùng đã like video chưa
            bool userLiked = false;
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                userLiked = await _likeRepository.HasUserLikedVideoAsync(userId, id);
            }

            // Lấy số lượng người đăng ký kênh
            var subscriberCount = await _subscriptionRepository.GetSubscriberCountAsync(video.UserId);

            // Kiểm tra người dùng đã đăng ký kênh chưa
            bool isSubscribed = false;
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                isSubscribed = await _subscriptionRepository.IsSubscribedAsync(userId, video.UserId);
            }

            var model = new VideoDetailViewModel
            {
                Video = video,
                RecommendedVideos = recommendedVideos,
                UserLiked = userLiked,
                SubscriberCount = subscriberCount,
                IsSubscribed = isSubscribed,
                Comments = video.Comments
            };

            // Truyền thông tin Cloudinary vào view
            ViewData["CloudinaryCloudName"] = _configuration["Cloudinary:CloudName"];

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddComment([FromBody] AddCommentRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Content))
            {
                return BadRequest(new { success = false, message = "Nội dung bình luận không được để trống" });
            }

            var comment = new Comment
            {
                VideoId = request.VideoId,
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                Content = request.Content,
                CreatedAt = DateTime.UtcNow
            };

            await _commentRepository.AddAsync(comment);

            // Trả về JSON cho client AJAX
            return Ok(new
            {
                success = true,
                comment = new
                {
                    avatarPath = comment.User?.AvatarPath, // Nếu có
                    userName = comment.User?.UserName,
                    userFullName = comment.User?.FullName,
                    content = comment.Content,
                    formattedCreatedAt = comment.CreatedAt
                }
            });
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ToggleLike(int videoId)
        {
            // Kiểm tra video có tồn tại không
            var video = await _videoRepository.GetByIdAsync(videoId);
            if (video == null)
            {
                return NotFound(new { success = false, message = "Video không tồn tại." });
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _likeRepository.ToggleLikeAsync(videoId, userId);

            // Có thể trả về JSON thay vì Redirect nếu dùng AJAX
            var likeCount = video.Likes?.Count(l => l.IsLike) ?? 0;
            var isLiked = await _likeRepository.HasUserLikedVideoAsync(userId, videoId);
            return Ok(new { success = true, likeCount, isLiked });
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Upload()
        {
            var categories = await _categoryRepository.GetAllAsync();
            var model = new UploadVideoViewModel
            {
                Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken] // Giữ lại vì AJAX có gửi token
        public async Task<IActionResult> Upload(UploadVideoViewModel model)
        {
            _logger.LogInformation("Upload POST method called for video title: {Title}", model?.Title ?? "N/A");

            if (model.VideoFile == null || model.VideoFile.Length == 0)
            {
                _logger.LogWarning("Upload attempt failed: Video file is missing.");
                return BadRequest(new { success = false, message = "Vui lòng chọn file video." });
            }
            // Xóa lỗi Categories nếu có (vì nó không được submit từ form AJAX theo cách thông thường)
            ModelState.Remove("Categories");
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Upload attempt failed due to invalid ModelState.");
                // Ghi log chi tiết lỗi ModelState
                foreach (var state in ModelState)
                {
                    if (state.Value.Errors.Count > 0)
                    {
                        _logger.LogWarning("ModelState Error in {Key}: {Errors}", state.Key, string.Join(", ", state.Value.Errors.Select(e => e.ErrorMessage)));
                    }
                }

                // Trả về lỗi BadRequest JSON chứa thông tin ModelState
                return BadRequest(new
                {
                    success = false,
                    message = "Dữ liệu nhập không hợp lệ. Vui lòng kiểm tra lại.", // Thông báo chung
                    errors = ModelState.Where(ms => ms.Value.Errors.Any())
                                     .ToDictionary(
                                         kvp => kvp.Key,
                                         kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                                     )
                });
            }

            // --- KIỂM TRA LẠI KÍCH THƯỚC FILE (Server-side validation) ---
            if (model.VideoFile.Length > 524288000) // Ví dụ: 500 MB - Nên lấy giá trị từ cấu hình
            {
                _logger.LogWarning("Upload attempt failed: Video file size ({Size} bytes) exceeds limit.", model.VideoFile.Length);
                return BadRequest(new { success = false, message = "Kích thước file video vượt quá giới hạn cho phép." });
            }
            if (model.ThumbnailFile != null && model.ThumbnailFile.Length > 2097152) // Ví dụ: 2 MB
            {
                _logger.LogWarning("Upload attempt failed: Thumbnail file size ({Size} bytes) exceeds limit.", model.ThumbnailFile.Length);
                return BadRequest(new { success = false, message = "Kích thước file thumbnail vượt quá giới hạn cho phép." });
            }


            // --- TIẾN HÀNH UPLOAD VÀ LƯU DATABASE ---
            try
            {
                // Sử dụng phương thức UploadVideoAsync từ VideoService
                _logger.LogInformation("Calling VideoService.UploadVideoAsync for video: {Title}", model.Title);
                int videoId = await _videoService.UploadVideoAsync(
                    model.Title,
                    model.Description,
                    model.CategoryId,
                    User.FindFirstValue(ClaimTypes.NameIdentifier),
                    model.VideoFile,
                    model.ThumbnailFile, // Truyền cả thumbnail (có thể null nếu không bắt buộc)
                    model.IsPublic);
                _logger.LogInformation("VideoService.UploadVideoAsync completed. Video ID: {VideoId}", videoId);

                // Xử lý tags nếu có và ITagService đã được inject
                if (!string.IsNullOrEmpty(model.Tags) && _tagService != null)
                {
                    _logger.LogInformation("Adding tags '{Tags}' to Video ID: {VideoId}", model.Tags, videoId);
                    await _tagService.AddTagsToVideoAsync(videoId, model.Tags);
                    _logger.LogInformation("Tags added successfully.");
                }
                else if (!string.IsNullOrEmpty(model.Tags) && _tagService == null)
                {
                    _logger.LogWarning("Tags provided ('{Tags}') but ITagService is not injected.", model.Tags);
                }


                // Trả về kết quả thành công dạng JSON
                return Ok(new { success = true, message = $"Video '{model.Title}' đã được tải lên thành công!" });
            }
            catch (Exception ex)
            {
                // Ghi log lỗi chi tiết
                _logger.LogError(ex, "Error occurred during video upload process for title '{Title}'.", model.Title);

                // Trả về lỗi Server Error dạng JSON
                return StatusCode(StatusCodes.Status500InternalServerError,
                                  new { success = false, message = "Đã có lỗi xảy ra trên máy chủ trong quá trình xử lý video. Vui lòng thử lại." });
            }
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> IncrementViewCount(int id)
        {
            await _videoService.IncrementViewCountAsync(id);
            return Ok(new { success = true });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            // Lấy thông tin video
            var video = await _videoRepository.GetByIdAsync(id);
            if (video == null)
            {
                return NotFound();
            }

            // Kiểm tra quyền sở hữu
            if (video.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier) && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            // Lấy danh sách tags của video
            var tagService = HttpContext.RequestServices.GetService(typeof(ITagService)) as ITagService;
            string tags = string.Empty;
            if (tagService != null)
            {
                var videoTags = await tagService.GetTagsByVideoIdAsync(id);
                tags = string.Join(", ", videoTags.Select(t => t.Name));
            }

            // Tạo view model
            var model = new EditVideoViewModel
            {
                Id = video.Id,
                Title = video.Title,
                Description = video.Description,
                CategoryId = video.CategoryId,
                Tags = tags,
                IsPublic = video.Status == VideoStatus.Public, // Chuyển đổi Status thành IsPublic
                CurrentVideoUrl = video.CloudinaryUrl, // Sử dụng CloudinaryUrl
                CurrentThumbnailUrl = video.ThumbnailUrl,
                CloudinaryPublicId = video.CloudinaryPublicId,
                Duration = video.Duration.TotalSeconds, // Chuyển đổi TimeSpan thành seconds
                CreatedAt = video.CreatedAt,
                ViewCount = video.ViewCount,
                VideoQuality = "auto", // Giá trị mặc định
                VideoFormat = "auto" // Giá trị mặc định
            };

            // Lấy danh sách danh mục
            var categories = await _categoryRepository.GetAllAsync();
            model.Categories = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name,
                Selected = c.Id == video.CategoryId
            });

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditVideoViewModel model)
        {
            // Xóa lỗi Categories khỏi ModelState
            ModelState.Remove("Categories");

            if (!ModelState.IsValid)
            {
                // Ghi log lỗi ModelState
                foreach (var state in ModelState)
                {
                    if (state.Value.Errors.Count > 0)
                    {
                        Console.WriteLine($"Error in {state.Key}: {string.Join(", ", state.Value.Errors.Select(e => e.ErrorMessage))}");
                    }
                }

                TempData["ErrorMessage"] = "Có lỗi xảy ra khi cập nhật video. Vui lòng kiểm tra lại thông tin.";

                // Lấy lại thông tin video
                var video = await _videoRepository.GetByIdAsync(model.Id);
                if (video == null)
                {
                    return NotFound();
                }

                // Cập nhật lại model
                model.CurrentVideoUrl = video.CloudinaryUrl;
                model.CurrentThumbnailUrl = video.ThumbnailUrl;
                model.CloudinaryPublicId = video.CloudinaryPublicId;
                model.Duration = video.Duration.TotalSeconds;
                model.CreatedAt = video.CreatedAt;
                model.ViewCount = video.ViewCount;

                // Lấy danh sách danh mục
                var categories = await _categoryRepository.GetAllAsync();
                model.Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name,
                    Selected = c.Id == model.CategoryId
                });

                return View(model);
            }

            try
            {
                // Lấy thông tin video hiện tại
                var video = await _videoRepository.GetByIdAsync(model.Id);
                if (video == null)
                {
                    return NotFound();
                }

                // Kiểm tra quyền sở hữu
                if (video.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier) && !User.IsInRole("Admin"))
                {
                    return Forbid();
                }

                // Cập nhật thông tin cơ bản
                video.Title = model.Title;
                video.Description = model.Description;
                video.CategoryId = model.CategoryId;
                video.Status = model.IsPublic ? VideoStatus.Public : VideoStatus.Private; // Chuyển đổi IsPublic thành Status
                video.UpdatedAt = DateTime.UtcNow;

                // Tải lên thumbnail mới nếu có
                if (model.ThumbnailFile != null)
                {
                    Console.WriteLine($"Uploading new thumbnail: {model.ThumbnailFile.FileName}");
                    var thumbnailResult = await _cloudinaryService.UploadImageAsync(model.ThumbnailFile);
                    video.ThumbnailUrl = thumbnailResult.Url;
                    video.ThumbnailPublicId = thumbnailResult.PublicId;
                    Console.WriteLine($"New thumbnail uploaded successfully. URL: {video.ThumbnailUrl}");
                }

                // Cập nhật video trong database
                await _videoRepository.UpdateAsync(video);
                Console.WriteLine($"Video updated in database. ID: {video.Id}");

                // Cập nhật tags nếu có
                if (model.Tags != null)
                {
                    var tagService = HttpContext.RequestServices.GetService(typeof(ITagService)) as ITagService;
                    if (tagService != null)
                    {
                        await tagService.UpdateTagsForVideoAsync(video.Id, model.Tags);
                        Console.WriteLine("Tags updated for video");
                    }
                }

                TempData["SuccessMessage"] = "Video đã được cập nhật thành công!";
                return RedirectToAction("Watch", new { id = video.Id });
            }
            catch (Exception ex)
            {
                // Ghi log lỗi chi tiết
                Console.WriteLine($"Error updating video: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");

                // Hiển thị thông báo lỗi chi tiết hơn
                ModelState.AddModelError("", $"Lỗi khi cập nhật video: {ex.Message}");
                TempData["ErrorMessage"] = $"Lỗi khi cập nhật video: {ex.Message}";

                // Lấy danh sách danh mục
                var categories = await _categoryRepository.GetAllAsync();
                model.Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name,
                    Selected = c.Id == model.CategoryId
                });

                return View(model);
            }
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var video = await _videoRepository.GetByIdAsync(id);
            if (video == null)
            {
                return NotFound();
            }

            // Kiểm tra quyền xóa
            if (User.FindFirstValue(ClaimTypes.NameIdentifier) != video.UserId)
            {
                return Forbid();
            }

            await _videoService.DeleteVideoAsync(id);

            return RedirectToAction("MyVideos", "Channel");
        }
        [Authorize]
        public async Task<IActionResult> Manage()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var videos = await _videoRepository.GetByUserIdAsync(userId);

            var videoWithAnalytics = new List<VideoWithAnalytics>();
            long totalViews = 0;
            long totalLikes = 0;
            long totalComments = 0;

            foreach (var video in videos)
            {
                VideoAnalytics analytics = null;

                // Lấy phân tích từ Cloudinary nếu có PublicId
                if (!string.IsNullOrEmpty(video.CloudinaryPublicId))
                {
                    try
                    {
                        var analyticsService = HttpContext.RequestServices.GetService<VideoAnalyticsService>();
                        if (analyticsService != null)
                        {
                            analytics = await analyticsService.GetVideoAnalyticsAsync(video.CloudinaryPublicId);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Bỏ qua lỗi khi lấy phân tích
                        Console.WriteLine($"Lỗi khi lấy phân tích: {ex.Message}");
                        analytics = new VideoAnalytics
                        {
                            PublicId = video.CloudinaryPublicId,
                            Views = video.ViewCount,
                            Bandwidth = 0,
                            LastAccessed = video.UpdatedAt ?? video.CreatedAt,
                            AccessMode = "public"
                        };
                    }
                }
                else
                {
                    // Tạo phân tích mặc định nếu không có PublicId
                    analytics = new VideoAnalytics
                    {
                        PublicId = "",
                        Views = video.ViewCount,
                        Bandwidth = 0,
                        LastAccessed = video.UpdatedAt ?? video.CreatedAt,
                        AccessMode = "public"
                    };
                }

                // Lấy số lượt like và comment
                int likeCount = video.Likes?.Count(l => l.IsLike) ?? 0;
                int commentCount = video.Comments?.Count() ?? 0;

                videoWithAnalytics.Add(new VideoWithAnalytics
                {
                    Video = video,
                    Analytics = analytics,
                    LikeCount = likeCount,
                    CommentCount = commentCount
                });

                totalViews += video.ViewCount;
                totalLikes += likeCount;
                totalComments += commentCount;
            }

            var model = new VideoManageViewModel
            {
                Videos = videoWithAnalytics,
                TotalVideos = videos.Count(),
                TotalViews = totalViews,
                TotalLikes = totalLikes,
                TotalComments = totalComments
            };

            return View(model);
        }
        [Authorize]
        public async Task<IActionResult> Analytics(int id)
        {
            var video = await _videoRepository.GetByIdAsync(id);
            if (video == null)
            {
                return NotFound();
            }

            // Kiểm tra quyền truy cập
            if (User.FindFirstValue(ClaimTypes.NameIdentifier) != video.UserId)
            {
                return Forbid();
            }

            VideoAnalytics analytics = null;

            // Lấy phân tích từ Cloudinary nếu có PublicId
            if (!string.IsNullOrEmpty(video.CloudinaryPublicId))
            {
                try
                {
                    var analyticsService = HttpContext.RequestServices.GetService<VideoAnalyticsService>();
                    if (analyticsService != null)
                    {
                        analytics = await analyticsService.GetVideoAnalyticsAsync(video.CloudinaryPublicId);
                    }
                }
                catch (Exception ex)
                {
                    // Bỏ qua lỗi khi lấy phân tích
                    Console.WriteLine($"Lỗi khi lấy phân tích: {ex.Message}");
                    analytics = new VideoAnalytics
                    {
                        PublicId = video.CloudinaryPublicId,
                        Views = video.ViewCount,
                        Bandwidth = 0,
                        LastAccessed = video.UpdatedAt ?? video.CreatedAt,
                        AccessMode = "public"
                    };
                }
            }
            else
            {
                // Tạo phân tích mặc định nếu không có PublicId
                analytics = new VideoAnalytics
                {
                    PublicId = "",
                    Views = video.ViewCount,
                    Bandwidth = 0,
                    LastAccessed = video.UpdatedAt ?? video.CreatedAt,
                    AccessMode = "public"
                };
            }

            // Lấy số lượt like và comment
            int likeCount = video.Likes?.Count(l => l.IsLike) ?? 0;
            int commentCount = video.Comments?.Count() ?? 0;

            var model = new VideoAnalyticsViewModel
            {
                Video = video,
                Analytics = analytics,
                LikeCount = likeCount,
                CommentCount = commentCount
            };

            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Search(string q)
        {
            if (string.IsNullOrEmpty(q))
            {
                return RedirectToAction("Index", "Home");
            }

            var videos = await _videoRepository.SearchAsync(q);
            return View(videos);
        }
    }
}
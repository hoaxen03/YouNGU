using YouNGU.Data.Repositories;
using YouNGU.Models.Entities;

namespace YouNGU.Services
{
    public class VideoService : IVideoService
    {
        private readonly IVideoRepository _videoRepository;
        private readonly CloudinaryService _cloudinaryService;
        private readonly INotificationService _notificationService;

        public VideoService(
            IVideoRepository videoRepository,
            CloudinaryService cloudinaryService,
            INotificationService notificationService)
        {
            _videoRepository = videoRepository;
            _cloudinaryService = cloudinaryService;
            _notificationService = notificationService;
        }

        public async Task<int> UploadVideoAsync(string title, string description, int categoryId, string userId, IFormFile videoFile, IFormFile thumbnailFile, bool isPublic)
        {
            // Kiểm tra file video
            if (videoFile == null || videoFile.Length == 0)
            {
                throw new ArgumentException("Vui lòng chọn file video");
            }

            // Kiểm tra định dạng video
            var allowedVideoExtensions = new[] { ".mp4", ".webm", ".mov", ".avi", ".wmv" };
            var videoExtension = Path.GetExtension(videoFile.FileName).ToLowerInvariant();
            if (!Array.Exists(allowedVideoExtensions, e => e == videoExtension))
            {
                throw new ArgumentException("Định dạng video không hỗ trợ. Vui lòng sử dụng MP4, WebM, MOV, AVI hoặc WMV.");
            }

            // Kiểm tra kích thước video (500MB)
            if (videoFile.Length > 524288000)
            {
                throw new ArgumentException("Kích thước video vượt quá giới hạn 500MB.");
            }
            // Quyết định StreamingProfile tự động
            string streamingProfile = videoExtension == ".mp4" ? "hls" : "dash";


            // Tải video lên Cloudinary 
            var videoUploadResult = await _cloudinaryService.UploadVideoAsync(videoFile, streamingProfile);


            // Xử lý thumbnail
            CloudinaryUploadResult thumbnailUploadResult = null;

            if (thumbnailFile != null && thumbnailFile.Length > 0)
            {
                // Kiểm tra định dạng thumbnail
                var allowedImageExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var imageExtension = Path.GetExtension(thumbnailFile.FileName).ToLowerInvariant();
                if (!Array.Exists(allowedImageExtensions, e => e == imageExtension))
                {
                    throw new ArgumentException("Định dạng hình ảnh không hỗ trợ. Vui lòng sử dụng JPG hoặc PNG.");
                }

                // Kiểm tra kích thước thumbnail (2MB)
                if (thumbnailFile.Length > 2097152)
                {
                    throw new ArgumentException("Kích thước hình ảnh vượt quá giới hạn 2MB.");
                }

                // Tải thumbnail lên Cloudinary
                thumbnailUploadResult = await _cloudinaryService.UploadThumbnailAsync(thumbnailFile);
            }

            // Tạo đối tượng Video
            var video = new Video
            {
                Title = title,
                Description = description,
                CloudinaryPublicId = videoUploadResult.PublicId,
                CloudinaryUrl = videoUploadResult.Url,
                Duration = TimeSpan.FromSeconds(videoUploadResult.Duration),
                ThumbnailPublicId = thumbnailUploadResult?.PublicId ?? "",
                ThumbnailUrl = thumbnailUploadResult?.SecureUrl ?? videoUploadResult.ThumbnailUrl,
                UserId = userId,
                CategoryId = categoryId,
                Status = isPublic ? VideoStatus.Published : VideoStatus.Private,
                CreatedAt = DateTime.UtcNow,
                StreamingProfile = streamingProfile,
                UpdatedAt = DateTime.UtcNow,
                ThumbnailPath = "default-thumbnail.jpg",
                VideoPath = "cloudinary",
                ViewCount = 0
            };

            // Lưu vào cơ sở dữ liệu
            await _videoRepository.AddAsync(video);

            return video.Id;
        }

        public async Task IncrementViewCountAsync(int videoId)
        {
            var video = await _videoRepository.GetByIdAsync(videoId);
            if (video != null)
            {
                video.ViewCount++;
                await _videoRepository.UpdateAsync(video);
            }
        }

        public async Task DeleteVideoAsync(int videoId)
        {
            var video = await _videoRepository.GetByIdAsync(videoId);
            if (video != null)
            {
                // Xóa video từ Cloudinary
                if (!string.IsNullOrEmpty(video.CloudinaryPublicId))
                {
                    await _cloudinaryService.DeleteResourceAsync(video.CloudinaryPublicId, "video");
                }

                // Xóa thumbnail từ Cloudinary (nếu không phải là thumbnail tự động)
                if (!string.IsNullOrEmpty(video.ThumbnailPublicId))
                {
                    await _cloudinaryService.DeleteResourceAsync(video.ThumbnailPublicId, "image");
                }

                // Xóa video từ cơ sở dữ liệu
                await _videoRepository.DeleteAsync(videoId);
            }
        }
    }
}

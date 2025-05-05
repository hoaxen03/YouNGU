using CloudinaryDotNet.Actions;
using YouNGU.Models.Entities;

namespace YouNGU.Services
{
    public interface IVideoService
    {
        Task<int> UploadVideoAsync(string title, string description, int categoryId, string userId, IFormFile videoFile, IFormFile thumbnailFile, bool isPublic);
        Task IncrementViewCountAsync(int videoId);
        Task DeleteVideoAsync(int videoId);
        // Phương thức mới - hỗ trợ Cloudinary
        /*
        Task<VideoUploadResult> UploadVideoAsync(
            IFormFile videoFile,
            IFormFile thumbnailFile,
            string quality = "auto",
            string format = "auto"
        );

        Task<VideoUploadResult> UploadVideoWithWatermarkAsync(
            IFormFile videoFile,
            IFormFile thumbnailFile,
            string quality = "auto",
            string format = "auto"
        );*/

    }

}

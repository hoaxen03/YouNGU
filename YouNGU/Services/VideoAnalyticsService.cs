using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using YouNGU.Models.ViewModels;

namespace YouNGU.Services
{
    public class VideoAnalyticsService
    {
        private readonly Cloudinary _cloudinary;

        public VideoAnalyticsService(IConfiguration configuration)
        {
            var account = new Account(
                configuration["Cloudinary:CloudName"],
                configuration["Cloudinary:ApiKey"],
                configuration["Cloudinary:ApiSecret"]);

            _cloudinary = new Cloudinary(account);
        }

        public async Task<VideoAnalytics> GetVideoAnalyticsAsync(string publicId)
        {
            if (string.IsNullOrEmpty(publicId))
                throw new ArgumentException("PublicId không được để trống");

            try
            {
                // Cố gắng lấy thông tin cơ bản từ Cloudinary
                var getResourceParams = new GetResourceParams(publicId)
                {
                    ResourceType = ResourceType.Video
                };
                var result = await _cloudinary.GetResourceAsync(getResourceParams);

                if (result.Error != null)
                {
                    throw new Exception($"Lỗi khi lấy thống kê video: {result.Error.Message}");
                }

                // Vì phiên bản CloudinaryDotNet không hỗ trợ các thuộc tính phân tích,
                // chúng ta sẽ sử dụng thông tin cơ bản có sẵn
                return new VideoAnalytics
                {
                    PublicId = result.PublicId,
                    Views = 0, // Không có thông tin lượt xem từ Cloudinary
                    Bandwidth = 0, // Không có thông tin băng thông từ Cloudinary
                    LastAccessed = DateTime.UtcNow, // Sử dụng thời gian hiện tại
                    AccessMode = result.Type // Sử dụng loại tài nguyên thay cho chế độ truy cập
                };
            }
            catch (Exception ex)
            {
                // Nếu có lỗi, trả về thông tin mặc định
                Console.WriteLine($"Lỗi khi lấy thống kê từ Cloudinary: {ex.Message}");

                return new VideoAnalytics
                {
                    PublicId = publicId,
                    Views = 0,
                    Bandwidth = 0,
                    LastAccessed = DateTime.UtcNow,
                    AccessMode = "public"
                };
            }
        }
    }
}

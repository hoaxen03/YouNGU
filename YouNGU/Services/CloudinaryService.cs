using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

// Services/CloudinaryService.cs

namespace YouNGU.Services
{
    public class CloudinaryService
    {
        private readonly Cloudinary _cloudinary;
        private readonly string _watermarkPublicId;

        public CloudinaryService(IConfiguration configuration)
        {
            var account = new Account(
                configuration["Cloudinary:CloudName"],
                configuration["Cloudinary:ApiKey"],
                configuration["Cloudinary:ApiSecret"]);

            _cloudinary = new Cloudinary(account);
            _cloudinary.Api.Secure = true;
            _watermarkPublicId = configuration["Cloudinary:WatermarkPublicId"];
        }
        public async Task<CloudinaryUploadResult> UploadImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is empty or null", nameof(file));
            }

            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "thumbnails"
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.Error != null)
            {
                throw new Exception($"Error uploading image to Cloudinary: {uploadResult.Error.Message}");
            }

            return new CloudinaryUploadResult
            {
                PublicId = uploadResult.PublicId,
                Url = uploadResult.SecureUrl.ToString(),
            };
        }

        public async Task<CloudinaryVideoUploadResult> UploadVideoAsync(IFormFile file,string streamingProfile , string quality = "auto", string format = "auto")
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is empty or null", nameof(file));
            }

            using var stream = file.OpenReadStream();
            var uploadParams = new VideoUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "videos",
                EagerTransforms = new List<Transformation>()
                {
                    new Transformation().StreamingProfile(streamingProfile)
                },
                EagerAsync = true
            };

            // Thêm transformation nếu cần
            if (quality != "auto" || format != "auto")
            {
                var transformation = new Transformation();

                if (quality != "auto")
                {
                    transformation = transformation.Quality(quality);
                }

                if (format != "auto")
                {
                    // Sử dụng SetFormat thay vì Format
                    transformation = transformation.FetchFormat(format);
                }

                uploadParams.Transformation = transformation;
            }

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.Error != null)
            {
                throw new Exception($"Error uploading video to Cloudinary: {uploadResult.Error.Message}");
            }

            // Tạo URL thumbnail từ video
            var thumbnailUrl = _cloudinary.Api.UrlImgUp.Transform(
                new Transformation().Width(640).Height(360).Crop("fill").Page("1")
            ).BuildUrl($"{uploadResult.PublicId}.jpg");

            return new CloudinaryVideoUploadResult
            {
                PublicId = uploadResult.PublicId,
                Url = uploadResult.SecureUrl.ToString(),
                ThumbnailUrl = thumbnailUrl,
                Duration = uploadResult.Duration
            };
        }


        public async Task<CloudinaryUploadResult> UploadThumbnailAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Không có file được chọn");

            using (var stream = file.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = "thumbnails"
                };

                // Thêm transformation cơ bản nếu có thể
                try
                {
                    uploadParams.Transformation = new Transformation().Width(640).Height(360).Crop("fill");
                }
                catch
                {
                    // Bỏ qua nếu không hỗ trợ
                }

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error != null)
                {
                    throw new Exception($"Lỗi khi tải lên thumbnail: {uploadResult.Error.Message}");
                }

                return new CloudinaryUploadResult
                {
                    PublicId = uploadResult.PublicId,
                    SecureUrl = uploadResult.SecureUrl.ToString(),
                    Format = uploadResult.Format,
                    ResourceType = uploadResult.ResourceType
                };
            }
        }

        public async Task DeleteResourceAsync(string publicId, string resourceType = "video")
        {
            if (string.IsNullOrEmpty(publicId))
                return;

            var deleteParams = new DeletionParams(publicId);

            // Thiết lập ResourceType dựa trên tham số
            if (resourceType == "video")
            {
                deleteParams.ResourceType = ResourceType.Video;
            }
            else if (resourceType == "image")
            {
                deleteParams.ResourceType = ResourceType.Image;
            }

            var result = await _cloudinary.DestroyAsync(deleteParams);

            if (result.Error != null)
            {
                throw new Exception($"Lỗi khi xóa resource: {result.Error.Message}");
            }
        }
        public async Task<CloudinaryAnalyticsResult> GetVideoAnalyticsAsync(string publicId)
        {
            if (string.IsNullOrEmpty(publicId))
            {
                throw new ArgumentException("Public ID is required", nameof(publicId));
            }

            try
            {
                var getResourceParams = new GetResourceParams(publicId)
                {
                    ResourceType = ResourceType.Video
                };

                var result = await _cloudinary.GetResourceAsync(getResourceParams);

                if (result.Error != null)
                {
                    throw new Exception($"Error getting video analytics: {result.Error.Message}");
                }

                var analytics = result.ImageMetadata;
                if (analytics == null)
                {
                    return new CloudinaryAnalyticsResult
                    {
                        Views = 0,
                        Plays = 0,
                        AveragePlayRate = 0,
                        AverageEngagement = 0
                    };
                }

                return new CloudinaryAnalyticsResult
                {
                    Views = int.TryParse(analytics.GetValueOrDefault("views"), out var views) ? views : 0,
                    Plays = int.TryParse(analytics.GetValueOrDefault("plays"), out var plays) ? plays : 0,
                    AveragePlayRate = double.TryParse(analytics.GetValueOrDefault("average_play_rate"), out var averagePlayRate) ? averagePlayRate : 0,
                    AverageEngagement = double.TryParse(analytics.GetValueOrDefault("average_engagement"), out var averageEngagement) ? averageEngagement : 0
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting video analytics: {ex.Message}", ex);
            }
        }

        public string GetVideoUrl(string publicId, int width = 640, int height = 360)
        {
            if (string.IsNullOrEmpty(publicId))
                return null;

            return _cloudinary.Api.UrlVideoUp.BuildUrl(publicId);
        }

        public string GetThumbnailUrl(string videoPublicId, int width = 640, int height = 360)
        {
            if (string.IsNullOrEmpty(videoPublicId))
                return null;

            return _cloudinary.Api.UrlImgUp.BuildUrl(videoPublicId);
        }
    }

    public class CloudinaryVideoUploadResult
    {
        public string PublicId { get; set; }
        public string Url { get; set; }
        public string ThumbnailUrl { get; set; }
        public double Duration { get; set; }
        public string Format { get; set; }
        public string ResourceType { get; set; }
        public string VideoPublicId { get; set; }
        public string VideoUrl { get; set; }

    }

    public class CloudinaryAnalyticsResult
    {
        public int Views { get; set; }
        public int Plays { get; set; }
        public double AveragePlayRate { get; set; }
        public double AverageEngagement { get; set; }
    }

    public class CloudinaryUploadResult
    {
        public string PublicId { get; set; }
        public string SecureUrl { get; set; }
        public string ThumbnailUrl { get; set; }
        public string Format { get; set; }
        public string ResourceType { get; set; }
        public double? Duration { get; set; }
        public string Url { get; internal set; }
        public string VideoPublicId { get; set; }
    }
}

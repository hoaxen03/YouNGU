using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace YouNGU.Models.ViewModels
{
    public class EditVideoViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tiêu đề video")]
        [StringLength(100, ErrorMessage = "Tiêu đề không được vượt quá 100 ký tự")]
        public string Title { get; set; }

        [StringLength(5000, ErrorMessage = "Mô tả không được vượt quá 5000 ký tự")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn danh mục")]
        [Display(Name = "Danh mục")]
        public int CategoryId { get; set; }

        [Display(Name = "Thẻ")]
        public string Tags { get; set; }

        [Display(Name = "Hình thumbnail mới")]
        public IFormFile ThumbnailFile { get; set; }

        [Display(Name = "Công khai")]
        public bool IsPublic { get; set; } = true;

        // Thông tin hiện tại của video
        public string CurrentVideoUrl { get; set; }
        public string CurrentThumbnailUrl { get; set; }
        public string CloudinaryPublicId { get; set; }
        public double Duration { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ViewCount { get; set; }
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }

        // Tùy chọn Cloudinary
        [Display(Name = "Chất lượng video")]
        public string VideoQuality { get; set; } = "auto";

        [Display(Name = "Định dạng video")]
        public string VideoFormat { get; set; } = "auto";

        [System.Text.Json.Serialization.JsonIgnore]
        public IEnumerable<SelectListItem> Categories { get; set; }

    }
}

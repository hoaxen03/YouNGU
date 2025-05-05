using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace YouNGU.Models.ViewModels
{
    public class UploadVideoViewModel
    {
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

        [Required(ErrorMessage = "Vui lòng chọn file video")]
        [Display(Name = "File video")]
        public IFormFile VideoFile { get; set; }

        [Display(Name = "Hình thumbnail")]
        public IFormFile ThumbnailFile { get; set; }

        [Display(Name = "Công khai")]
        public bool IsPublic { get; set; } = true;

        // Thêm [BindNever] để ngăn ModelState xác thực thuộc tính này
        [System.Text.Json.Serialization.JsonIgnore]
        [BindNever]
        public IEnumerable<SelectListItem> Categories { get; set; }

        // Tùy chọn Cloudinary
        [Display(Name = "Chất lượng video")]
        public string VideoQuality { get; set; } = "auto";

        [Display(Name = "Định dạng video")]
        public string VideoFormat { get; set; } = "auto";
    }
}

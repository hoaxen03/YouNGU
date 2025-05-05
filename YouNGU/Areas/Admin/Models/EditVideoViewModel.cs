using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using YouNGU.Models.Entities;

namespace YouNGU.Areas.Admin.Models
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
        public int CategoryId { get; set; }

        public VideoStatus Status { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }
        public bool IsPublic { get; internal set; }
    }
}

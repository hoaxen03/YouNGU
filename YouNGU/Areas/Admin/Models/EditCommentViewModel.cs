using System.ComponentModel.DataAnnotations;
using YouNGU.Models.Entities;

namespace YouNGU.Areas.Admin.Models
{
    public class EditCommentViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nội dung không được để trống")]
        [StringLength(1000, ErrorMessage = "Nội dung không được vượt quá 1000 ký tự")]
        public string Content { get; set; }

        public bool IsApproved { get; set; }

        public Comment Comment { get; set; }
    }
}

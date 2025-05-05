using System.ComponentModel.DataAnnotations;

namespace YouNGU.Models.ViewModels
{
    public class CreatePlaylistViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên playlist")]
        [StringLength(100, ErrorMessage = "Tên playlist không được vượt quá 100 ký tự")]
        public string Name { get; set; }

        public bool IsPublic { get; set; } = true;

    }
}

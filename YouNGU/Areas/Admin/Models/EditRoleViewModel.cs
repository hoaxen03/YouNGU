using System.ComponentModel.DataAnnotations;

namespace YouNGU.Areas.Admin.Models
{
    public class EditRoleViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Tên vai trò không được để trống")]
        [StringLength(50, ErrorMessage = "Tên vai trò không được vượt quá 50 ký tự")]
        public string Name { get; set; }
    }
}

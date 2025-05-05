using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace YouNGU.Areas.Admin.Models
{
    public class EditUserViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên người dùng")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        public string FullName { get; set; }

        public IEnumerable<string> UserRoles { get; set; }

        public IEnumerable<IdentityRole> AllRoles { get; set; }

    }
}

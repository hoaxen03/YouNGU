using System.ComponentModel.DataAnnotations;

namespace YouNGU.Areas.Admin.Models
{
    public class SettingsViewModel
    {
        [Required(ErrorMessage = "Tên trang web không được để trống")]
        [StringLength(100, ErrorMessage = "Tên trang web không được vượt quá 100 ký tự")]
        public string SiteName { get; set; }

        [StringLength(500, ErrorMessage = "Mô tả trang web không được vượt quá 500 ký tự")]
        public string SiteDescription { get; set; }

        [Required(ErrorMessage = "Email liên hệ không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string ContactEmail { get; set; }

        [Range(1, 2000, ErrorMessage = "Kích thước tải lên tối đa phải từ 1MB đến 2000MB")]
        public int MaxUploadSizeMB { get; set; }

        [Required(ErrorMessage = "Phần mở rộng video cho phép không được để trống")]
        public string AllowedVideoExtensions { get; set; }

        public bool EnableComments { get; set; }

        public bool RequireCommentApproval { get; set; }

        public bool EnableUserRegistration { get; set; }

        [Required(ErrorMessage = "Vai trò người dùng mặc định không được để trống")]
        public string DefaultUserRole { get; set; }
    }
}

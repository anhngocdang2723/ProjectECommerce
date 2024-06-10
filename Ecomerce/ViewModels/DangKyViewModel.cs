using System.ComponentModel.DataAnnotations;

namespace ECommerce.ViewModels
{
    public class DangKyViewModel
    {
        [Display(Name = "Tên đăng nhập")]
        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        [MaxLength(20, ErrorMessage = "Tên đăng nhập không được quá 20 ký tự")]
        public string MaKh { get; set; }

        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [DataType(DataType.Password)]
        public string MatKhau { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }
    }
}

public class DangNhapViewModel
{
    [Display(Name = "Tên đăng nhập")]
    [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
    [MaxLength(20, ErrorMessage = "Tên đăng nhập không được quá 20 ký tự")]
    public string Username { get; set; }

    [Display(Name = "Mật khẩu")]
    [Required(ErrorMessage = "Mật khẩu không được để trống")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Display(Name = "Ghi nhớ tôi")]
    public bool RememberMe { get; set; }
}

public class XacThucEmailViewModel
{
    [Required(ErrorMessage = "Mã xác thực không được để trống")]
    public string VerificationCode { get; set; }
    public string MaKh { get; internal set; }
    public object MaKH { get; internal set; }
    public string Email { get; internal set; }
}

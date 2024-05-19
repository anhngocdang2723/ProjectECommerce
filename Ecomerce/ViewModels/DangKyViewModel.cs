using System.ComponentModel.DataAnnotations;

namespace ECommerce.ViewModels
{
    public class DangKyViewModel
    {
        [Display(Name = "Tên đăng nhập")]
        [Required(ErrorMessage = "Mã khách hàng không được để trống")]
        [MaxLength(20, ErrorMessage = "Mã khách hàng không được quá 20 ký tự")]
        public string MaKh { get; set; }

        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        public string MatKhau { get; set; }

        [Display(Name = "Họ tên")]
        [MaxLength(50, ErrorMessage = "Họ tên không được quá 50 ký tự")]
        public string HoTen { get; set; }

        public bool GioiTinh { get; set; }

        public DateTime? NgaySinh { get; set; }
        [MaxLength(60, ErrorMessage = "Địa chỉ không được quá 60 ký tự")]
        public string DiaChi { get; set; }
        [MaxLength(24, ErrorMessage = "SĐT không được quá 24 ký tự")]
        [RegularExpression(@"^0\d{9,10}$", ErrorMessage = "Số điện thoại không đúng định dạng")]
        public string DienThoai { get; set; }
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; }

        public string? Hinh { get; set; }
    }
}

using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.ViewModels
{
    public class HangHoaViewModel
    {
        public int MaHh { get; set; }
        public required string TenHh { get; set; }
        public decimal DonGia { get; set; }
        public string? Hinh { get; set; }
        public required string MoTaNgan { get; set; }
        public required string TenLoai { get; set; }
        public string? MaNCC { get; set; }
        public int SoLuong { get; set; }
        public IFormFile ImageFile { get; set; }
    }

    public class ChiTietHangHoaViewModel
    {
        public int MaHh { get; set; }
        public required string TenHh { get; set; }
        public decimal DonGia { get; set; }
        public string? Hinh { get; set; }
        public required string MoTaNgan { get; set; }
        public required string TenLoai { get; set; }
        public required string ChiTiet { get; set; }
        public string? MaNCC { get; set; }
        public int DiemDanhGia { get; set; }
        public int SoLuongTonKho { get; set; }
    }
}

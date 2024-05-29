using System.Collections.Generic;

namespace ECommerce.ViewModels
{
    public class ThanhToanViewModel
    {
        public string HoTen { get; set; }
        public string DiaChi { get; set; }
        public string DienThoai { get; set; }
        public string GhiChu { get; set; }
        public double PhiVanChuyen { get; set; } = 15;
        public List<GioHangItem> GioHangItems { get; set; } = new List<GioHangItem>();
    }
}

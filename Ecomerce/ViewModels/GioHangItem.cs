namespace ECommerce.ViewModels
{
    public class GioHangItem
    {
        public int MaHh { get; set; }
        public required string Hinh { get; set; }
        public required string TenHh { get; set; }
        public double DonGia { get; set; }
        public int SoLuong { get; set; } // Số lượng sản phẩm có sẵn trong kho
        public int SoLuongMua { get; set; } // Số lượng sản phẩm người dùng muốn mua
        public double ThanhTien
        {
            get { return SoLuongMua * DonGia; }
            set { }
        }
    }
}

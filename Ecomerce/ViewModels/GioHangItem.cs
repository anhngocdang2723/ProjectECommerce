namespace ECommerce.ViewModels
{
    public class GioHangItem
    {
        public int MaHh { get; set; }
        public string TenHh { get; set; }
        public string Hinh { get; set; }
        public double DonGia { get; set; }
        public int SoLuong { get; set; }
        public int SoLuongMua { get; set; }
        public double ThanhTien => SoLuongMua * DonGia;
    }

}

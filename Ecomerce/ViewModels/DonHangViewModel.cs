namespace ECommerce.ViewModels
{
    public class DonHangViewModel
    {
        public int MaHd { get; set; }
        public DateTime NgayDat { get; set; }
        public double TongTien { get; set; }  // Sử dụng double thay vì decimal
        public string TrangThai { get; set; }
    }
}

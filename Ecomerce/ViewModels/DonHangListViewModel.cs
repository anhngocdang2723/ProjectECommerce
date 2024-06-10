namespace ECommerce.ViewModels
{
    public class DonHangListViewModel
    {
        public List<DonHangViewModel> DonHangs { get; set; }
        public int TotalOrders { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }
}

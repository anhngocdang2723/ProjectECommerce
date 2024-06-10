namespace ECommerce.ViewModels
{
    public class HangHoaListViewModel
    {
        public List<HangHoaViewModel> HangHoas { get; set; }
        public int TotalHangHoas { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

}

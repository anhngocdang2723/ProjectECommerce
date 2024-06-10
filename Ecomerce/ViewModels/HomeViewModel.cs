namespace ECommerce.ViewModels
{
    public class HomeViewModel
    {
        public List<HangHoaViewModel> TopInteractedProducts { get; set; }
        public List<HangHoaViewModel> TopLaptops { get; set; }
        public List<HangHoaViewModel> TopDienthoais { get; internal set; }
        public List<HangHoaViewModel> TopPhones { get; internal set; }
    }
}

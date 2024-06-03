using ECommerce.Data;
using ECommerce.Models;
using System.Collections.Generic;

namespace ECommerce.ViewModels
{
    public class UserListViewModel
    {
        public List<KhachHang> Users { get; set; }
        public int TotalUsers { get; set; }
        public string SearchField { get; set; }
        public string SearchText { get; set; }
        public string StatusFilter { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }
}

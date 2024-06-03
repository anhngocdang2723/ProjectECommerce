// OrderListViewModel.cs
using ECommerce.Data;
using System;
using System.Collections.Generic;

namespace ECommerce.ViewModels
{
    public class OrderListViewModel
    {
        public IEnumerable<HoaDon> Orders { get; set; }
        public int TotalOrders { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public required string SearchField { get; set; }
        public string SearchText { get; set; }
        public string DateFilter { get; set; }
    }
}

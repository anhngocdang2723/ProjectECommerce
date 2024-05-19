using ECommerce.Helpers;
using ECommerce.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.ViewComponents
{
    public class GioHangViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var gioHang = HttpContext.Session.Get<List<GioHangItem>>(MyConst.GIO_HANG) ?? new List<GioHangItem>();
            return View(new GioHangModel
                {
                SoLuong = gioHang.Sum(p => p.SoLuong),
                Tong = gioHang.Sum(p => p.ThanhTien)
            });
        }
    }
}

using ECommerce.ViewModels;
using ECommerce.Data;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.ViewComponents
{
    public class MenuLoaiViewComponent : ViewComponent
    {
        private readonly Hshop2023Context db;

        public MenuLoaiViewComponent(Hshop2023Context context) => db = context;

        public IViewComponentResult Invoke()
        {
            var data = db.Loais.Select(lo => new MenuLoaiViewModel
            {
                MaLoai = lo.MaLoai,
                TenLoai = lo.TenLoai,
                SoLuong = lo.HangHoas.Count
            }).OrderBy(keySelector: p => p.TenLoai);

            return View("Default", data);
        }
    }
}

using ECommerce.Data;
using Microsoft.AspNetCore.Mvc;
using ECommerce.Helpers;
using ECommerce.ViewModels;

namespace ECommerce.Controllers
{
    public class GioHangController : Controller
    {
        public readonly Hshop2023Context db;
        public GioHangController(Hshop2023Context context)
        {
            this.db = context;
        }

        public List<GioHangItem> GioHangItems
        {
            get
            {
                var gioHang = HttpContext.Session.Get<List<GioHangItem>>(MyConst.GIO_HANG);
                if (gioHang == null)
                {
                    gioHang = new List<GioHangItem>();
                    HttpContext.Session.Set(MyConst.GIO_HANG, gioHang);
                }
                return gioHang;
            }
        }

        public IActionResult Index()
        {
            return View(GioHangItems);
        }
        public IActionResult ThemVaoGioHang(int id, int soluong = 1)
        {
            var giohang = GioHangItems;
            var item = giohang.SingleOrDefault(p => p.MaHh == id);
            if (item == null)
            {
                var hanghoa = db.HangHoas.SingleOrDefault(p => p.MaHh == id);
                if (hanghoa == null)
                {
                    TempData["Message"] = $"Không tìm thấy sản phẩm có mã {id}";
                    return Redirect("/404");
                }
                item = new GioHangItem
                {
                    MaHh = hanghoa.MaHh,
                    Hinh = hanghoa.Hinh ?? "",
                    TenHh = hanghoa.TenHh,
                    DonGia = (double)(hanghoa.DonGia ?? 0),
                    SoLuong = soluong
                };
                giohang.Add(item);
            }
            else
            {
                item.SoLuong += soluong;
            }
            HttpContext.Session.Set(MyConst.GIO_HANG, giohang);
            return RedirectToAction("index");
        }

        public IActionResult XoaKhoiGioHang (int id)
        {
            var giohang = GioHangItems;
            var item = giohang.SingleOrDefault(p => p.MaHh == id);
            if (item != null)
            {
                giohang.Remove(item);
            }
            HttpContext.Session.Set(MyConst.GIO_HANG, giohang);
            return RedirectToAction("index");
        }
    }
}

using ECommerce.Data;
using ECommerce.Helpers;
using ECommerce.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace ECommerce.Controllers
{
    public class GioHangController : Controller
    {
        private readonly Hshop2023Context _context;

        public GioHangController(Hshop2023Context context)
        {
            _context = context;
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
            var gioHang = GioHangItems;
            var item = gioHang.SingleOrDefault(p => p.MaHh == id);
            var hangHoa = _context.HangHoas.SingleOrDefault(p => p.MaHh == id);

            if (hangHoa == null)
            {
                TempData["Message"] = $"Không tìm thấy sản phẩm có mã {id}";
                return Redirect("/404");
            }

            if (item == null)
            {
                if (hangHoa.SoLuong < soluong)
                {
                    TempData["Message"] = "Số lượng sản phẩm trong kho không đủ";
                    return RedirectToAction("Index");
                }

                item = new GioHangItem
                {
                    MaHh = hangHoa.MaHh,
                    Hinh = hangHoa.Hinh ?? "",
                    TenHh = hangHoa.TenHh,
                    DonGia = (double)(hangHoa.DonGia ?? 0),
                    SoLuongMua = soluong,
                    SoLuong = hangHoa.SoLuong
                };
                gioHang.Add(item);
            }
            else
            {
                if (item.SoLuongMua + soluong > hangHoa.SoLuong)
                {
                    TempData["Message"] = "Số lượng sản phẩm trong kho không đủ";
                    return RedirectToAction("Index");
                }

                item.SoLuongMua += soluong;
            }

            HttpContext.Session.Set(MyConst.GIO_HANG, gioHang);
            return RedirectToAction("Index");
        }

        public IActionResult XoaKhoiGioHang(int id)
        {
            var gioHang = GioHangItems;
            var item = gioHang.SingleOrDefault(p => p.MaHh == id);
            if (item != null)
            {
                gioHang.Remove(item);
            }
            HttpContext.Session.Set(MyConst.GIO_HANG, gioHang);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult CapNhatSoLuong(int id, int soluong)
        {
            var gioHang = GioHangItems;
            var item = gioHang.SingleOrDefault(p => p.MaHh == id);
            var hangHoa = _context.HangHoas.SingleOrDefault(p => p.MaHh == id);

            if (item != null && hangHoa != null)
            {
                if (soluong > hangHoa.SoLuong)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Số lượng sản phẩm trong kho không đủ"
                    });
                }

                item.SoLuongMua = soluong;
            }
            HttpContext.Session.Set(MyConst.GIO_HANG, gioHang);
            return Json(new
            {
                success = true,
                totalPrice = item.ThanhTien,
                subtotal = gioHang.Sum(i => i.ThanhTien)
            });
        }
    }
}

using ECommerce.Data;
using ECommerce.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Controllers
{
    public class HangHoaController : Controller
    {
        public readonly Hshop2023Context db;
        public HangHoaController(Hshop2023Context context)
        {
            this.db = context;
        }
        public IActionResult Index(int? loai)
        {
            var hanghoas = db.HangHoas.AsQueryable();
            if (loai.HasValue)
            {
                hanghoas = hanghoas.Where(p => p.MaLoai == loai.Value);
            }
            var result = hanghoas.Select(p => new HangHoaViewModel
            {
                MaHh = p.MaHh,
                TenHh = p.TenHh,
                DonGia = (decimal)(p.DonGia ?? 0),
                Hinh = p.Hinh ?? "",
                MoTaNgan = p.MoTaDonVi ?? "",
                TenLoai = p.MaLoaiNavigation.TenLoai
            });
            return View(result);
        }
        public IActionResult Search(string query)
        {
            var hanghoas = db.HangHoas.AsQueryable();
            if (query != null)
            {
                hanghoas = hanghoas.Where(p => p.TenHh.Contains(query));
            }
            var result = hanghoas.Select(p => new HangHoaViewModel
            {
                MaHh = p.MaHh,
                TenHh = p.TenHh,
                DonGia = (decimal)(p.DonGia ?? 0),
                Hinh = p.Hinh ?? "",
                MoTaNgan = p.MoTaDonVi ?? "",
                TenLoai = p.MaLoaiNavigation.TenLoai
            });
            return View(result);
        }
        public IActionResult Detail(int id)
        {
            var data = db.HangHoas
                .Include(p => p.MaLoaiNavigation)
                .SingleOrDefault(p => p.MaHh == id);
            if (data == null)
            {
                TempData["Message"] = $"Không tìm thấy sản phẩm có mã {id}";
                return Redirect("/404");
            }
            var result = new ChiTietHangHoaViewModel
            {
                MaHh = data.MaHh,
                TenHh = data.TenHh,
                DonGia = (decimal)(data.DonGia ?? 0),
                Hinh = data.Hinh ?? string.Empty,
                MoTaNgan = data.MoTaDonVi ?? string.Empty,
                TenLoai = data.MaLoaiNavigation.TenLoai,
                ChiTiet = data.MoTa ?? string.Empty,
                DiemDanhGia = 5, //check sau
                SoLuongTonKho = 10 //check sau
            };
            return View(result);
        }
    }
}

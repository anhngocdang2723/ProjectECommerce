using ECommerce.Data;
using ECommerce.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Controllers
{
    public class HangHoaController : Controller
    {
        private readonly Hshop2023Context db;

        public HangHoaController(Hshop2023Context context)
        {
            db = context;
        }

        public IActionResult Search(string query)
        {
            var hanghoas = db.HangHoas.Where(hh => hh.SoLuong > 0).AsQueryable();
            if (!string.IsNullOrEmpty(query))
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

            data.SoLuongTuongTac++;
            db.SaveChanges();

            var result = new ChiTietHangHoaViewModel
            {
                MaHh = data.MaHh,
                TenHh = data.TenHh,
                DonGia = (decimal)(data.DonGia ?? 0),
                Hinh = data.Hinh ?? string.Empty,
                MoTaNgan = data.MoTaDonVi ?? string.Empty,
                TenLoai = data.MaLoaiNavigation.TenLoai,
                ChiTiet = data.MoTa ?? string.Empty,
                DiemDanhGia = 5,
                SoLuongTonKho = (int)data.SoLuong
            };

            return View(result);
        }


        public async Task<IActionResult> Index(int? loai, int pageIndex = 1, int pageSize = 12)
        {
            ViewData["PageSize"] = pageSize;

            var hanghoas = db.HangHoas.Where(hh => hh.SoLuong > 0).AsQueryable();
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

            var paginatedList = ChiaTrangSPViewModel<HangHoaViewModel>.Create(result, pageIndex, pageSize);
            return View(paginatedList);
        }

    }
}

using ECommerce.Data;
using ECommerce.Models;
using ECommerce.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Linq;

namespace ECommerce.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Hshop2023Context _context;

        public HomeController(ILogger<HomeController> logger, Hshop2023Context context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var topInteractedProducts = _context.HangHoas
                .OrderByDescending(p => p.SoLuongTuongTac)
                .Where(hh => hh.SoLuong > 0)
                .Take(8)
                .Select(p => new HangHoaViewModel
                {
                    MaHh = p.MaHh,
                    TenHh = p.TenHh,
                    DonGia = (decimal)(p.DonGia ?? 0),
                    Hinh = p.Hinh ?? "",
                    MoTaNgan = p.MoTaDonVi ?? "",
                    TenLoai = p.MaLoaiNavigation.TenLoai
                }).ToList();

            var topLaptops = _context.HangHoas
                .Where(hh => hh.MaLoaiNavigation.TenLoai == "Laptop" && hh.SoLuong > 0)
                .OrderByDescending(p => p.SoLuongTuongTac)
                .Take(4)
                .Select(p => new HangHoaViewModel
                {
                    MaHh = p.MaHh,
                    TenHh = p.TenHh,
                    DonGia = (decimal)(p.DonGia ?? 0),
                    Hinh = p.Hinh ?? "",
                    MoTaNgan = p.MoTaDonVi ?? "",
                    TenLoai = p.MaLoaiNavigation.TenLoai
                }).ToList();

            var topPhones = _context.HangHoas
                .Where(hh => hh.MaLoaiNavigation.TenLoai == "Điện thoại" && hh.SoLuong > 0)
                .OrderByDescending(p => p.SoLuongTuongTac)
                .Take(4)
                .Select(p => new HangHoaViewModel
                {
                    MaHh = p.MaHh,
                    TenHh = p.TenHh,
                    DonGia = (decimal)(p.DonGia ?? 0),
                    Hinh = p.Hinh ?? "",
                    MoTaNgan = p.MoTaDonVi ?? "",
                    TenLoai = p.MaLoaiNavigation.TenLoai
                }).ToList();

            var model = new HomeViewModel
            {
                TopInteractedProducts = topInteractedProducts,
                TopLaptops = topLaptops,
                TopPhones = topPhones
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("/404")]
        public IActionResult PageNotFound()
        {
            return View();
        }
    }
}

using ECommerce.Data;
using ECommerce.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Controllers
{
    [Route("admin/hanghoa")]
    [Authorize(Roles = "Admin")]
    public class AdminHangHoaController : Controller
    {
        private readonly Hshop2023Context _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminHangHoaController(Hshop2023Context context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        private string UploadFile(IFormFile file)
        {
            string uniqueFileName = null;

            if (file != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "wwwroot/img/HangHoa");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }
        #region Index
        [HttpGet("index")]
        public IActionResult Index(int pageNumber = 1, int pageSize = 20)
        {

            var hangHoas = _context.HangHoas
                .Select(hh => new HangHoaViewModel
                {
                    MaHh = hh.MaHh,
                    TenHh = hh.TenHh,
                    DonGia = (decimal)(hh.DonGia ?? 0),
                    Hinh = hh.Hinh ?? "",
                    MoTaNgan = hh.MoTaDonVi ?? "",
                    TenLoai = hh.MaLoaiNavigation.TenLoai,
                    SoLuong = hh.SoLuong ?? 0
                })
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .OrderBy(hh => hh.SoLuong)
                .ToList();

            var totalHangHoas = _context.HangHoas.Count();

            var model = new HangHoaListViewModel
            {
                HangHoas = hangHoas,
                TotalHangHoas = totalHangHoas,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return View(model);
        }
        #endregion

        #region Create
        [HttpGet("create")]
        public IActionResult Create()
        {
            ViewBag.LoaiSanPhams = _context.Loais.ToList();
            return View();
        }

        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(HangHoaViewModel model)
        {
            if(model.SoLuong <0)
            {
                ModelState.AddModelError("SoLuong", "Số lượng không thể nhỏ hơn 0");
            }
            if (ModelState.IsValid)
            {
                var hangHoa = new HangHoa
                {
                    TenHh = model.TenHh,
                    DonGia = (double?)model.DonGia,
                    Hinh = null,
                    MoTaDonVi = model.MoTaNgan,
                    SoLuong = model.SoLuong,
                    MaLoai = _context.Loais.FirstOrDefault(l => l.TenLoai == model.TenLoai)?.MaLoai ?? 0,
                    MaNcc = "SS" 
                };

                _context.HangHoas.Add(hangHoa);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.LoaiSanPhams = _context.Loais.ToList();
            return View(model);
        }

        #endregion        
        #region Edit
        [HttpGet("edit/{id}")]
        public IActionResult Edit(int id)
        {
            var hangHoa = _context.HangHoas.Include(h => h.MaLoaiNavigation).FirstOrDefault(h => h.MaHh == id);
            if (hangHoa == null)
            {
                return NotFound();
            }

            var model = new HangHoaViewModel
            {
                MaHh = hangHoa.MaHh,
                TenHh = hangHoa.TenHh,
                DonGia = (decimal)(hangHoa.DonGia ?? 0),
                Hinh = hangHoa.Hinh ?? "",
                MoTaNgan = hangHoa.MoTaDonVi ?? "",
                TenLoai = hangHoa.MaLoaiNavigation.TenLoai,
                SoLuong = hangHoa.SoLuong ?? 0
            };

            ViewBag.LoaiSanPhams = _context.Loais.ToList();
            return View(model);
        }

        [HttpPost("edit/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, HangHoaViewModel model, IFormFile ImageFile)
        {
            if (ModelState.IsValid)
            {
                var hangHoa = _context.HangHoas.Find(id);
                if (hangHoa == null)
                {
                    return NotFound();
                }

                hangHoa.TenHh = model.TenHh;
                hangHoa.DonGia = (double?)model.DonGia;
                hangHoa.Hinh = string.IsNullOrEmpty(UploadFile(ImageFile)) ? hangHoa.Hinh : UploadFile(ImageFile);
                hangHoa.MoTaDonVi = model.MoTaNgan;
                hangHoa.SoLuong = model.SoLuong;
                hangHoa.MaLoai = _context.Loais.FirstOrDefault(l => l.TenLoai == model.TenLoai)?.MaLoai ?? 0;

                _context.HangHoas.Update(hangHoa);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.LoaiSanPhams = _context.Loais.ToList();
            return View(model);
        }
        #endregion
        #region Delete
        [HttpPost("delete/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var hangHoa = _context.HangHoas.Find(id);
            if (hangHoa == null)
            {
                return NotFound();
            }

            _context.HangHoas.Remove(hangHoa);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        #endregion
        #region Detail
        [HttpGet("details/{id}")]
        public IActionResult Details(int id)
        {
            var hangHoa = _context.HangHoas
                .Include(h => h.MaLoaiNavigation)
                .FirstOrDefault(h => h.MaHh == id);
            if (hangHoa == null)
            {
                return NotFound();
            }

            var model = new ChiTietHangHoaViewModel
            {
                MaHh = hangHoa.MaHh,
                TenHh = hangHoa.TenHh,
                DonGia = (decimal)(hangHoa.DonGia ?? 0),
                Hinh = hangHoa.Hinh ?? "",
                MoTaNgan = hangHoa.MoTaDonVi ?? "",
                TenLoai = hangHoa.MaLoaiNavigation.TenLoai,
                ChiTiet = hangHoa.MoTa ?? "",
                DiemDanhGia = 5, // Placeholder for now
                SoLuongTonKho = hangHoa.SoLuong ?? 0
            };

            return View(model);
        }
        #endregion
    }
}

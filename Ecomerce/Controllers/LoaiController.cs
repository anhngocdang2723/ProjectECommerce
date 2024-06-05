using ECommerce.Data;
using ECommerce.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace ECommerce.Controllers
{
    [Route("admin/[controller]/[action]")]
    public class LoaiController : Controller
    {
        private readonly Hshop2023Context _context;

        public LoaiController(Hshop2023Context context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var loaiList = _context.Loais.Select(l => new LoaiViewModel
            {
                MaLoai = l.MaLoai,
                TenLoai = l.TenLoai,
                TenLoaiAlias = l.TenLoaiAlias,
                MoTa = l.MoTa,
                Hinh = l.Hinh
            }).ToList();

            return View(loaiList);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(LoaiViewModel model)
        {
            if (ModelState.IsValid)
            {
                var loai = new Loai
                {
                    TenLoai = model.TenLoai,
                    TenLoaiAlias = model.TenLoaiAlias,
                    MoTa = model.MoTa,
                    Hinh = model.Hinh
                };
                _context.Loais.Add(loai);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var loai = _context.Loais.Find(id);
            if (loai == null)
            {
                return NotFound();
            }

            var model = new LoaiViewModel
            {
                MaLoai = loai.MaLoai,
                TenLoai = loai.TenLoai,
                TenLoaiAlias = loai.TenLoaiAlias,
                MoTa = loai.MoTa,
                Hinh = loai.Hinh
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(LoaiViewModel model)
        {
            if (ModelState.IsValid)
            {
                var loai = _context.Loais.Find(model.MaLoai);
                if (loai == null)
                {
                    return NotFound();
                }

                loai.TenLoai = model.TenLoai;
                loai.TenLoaiAlias = model.TenLoaiAlias;
                loai.MoTa = model.MoTa;
                loai.Hinh = model.Hinh;

                _context.Loais.Update(loai);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var loai = _context.Loais.Find(id);
            if (loai == null)
            {
                return NotFound();
            }

            _context.Loais.Remove(loai);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}

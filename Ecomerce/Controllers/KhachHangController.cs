using ECommerce.Data;
using ECommerce.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ECommerce.Controllers
{
    public class KhachHangController : Controller
    {
        private readonly Hshop2023Context db;

        public KhachHangController(Hshop2023Context context)
        {
            db = context;
        }

        #region DangKy
        [HttpGet]
        public IActionResult DangKy()
        {
            return View();
        }

        [HttpPost]
        public IActionResult DangKy(DangKyViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Mã hóa mật khẩu bằng MD5
                string hashedPassword = MD5Helper.GetMd5Hash(model.MatKhau);

                KhachHang kh = new KhachHang
                {
                    MaKh = model.MaKh,
                    MatKhau = hashedPassword,
                    HoTen = model.HoTen,
                    GioiTinh = model.GioiTinh,
                    NgaySinh = (DateTime)model.NgaySinh,
                    DiaChi = model.DiaChi,
                    DienThoai = model.DienThoai,
                    Email = model.Email,
                    Hinh = model.Hinh
                };
                db.KhachHangs.Add(kh);
                db.SaveChanges();
                return RedirectToAction("DangNhap", "TaiKhoan");
            }
            return View(model);
        }
        #endregion

        #region DangNhap
        [HttpGet]
        public IActionResult DangNhap(string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DangNhap(DangNhapViewModel model, string? ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                // Mã hóa mật khẩu bằng MD5
                string hashedPassword = MD5Helper.GetMd5Hash(model.Password);

                KhachHang kh = db.KhachHangs.SingleOrDefault(k => k.MaKh == model.Username && k.MatKhau == hashedPassword);
                if (kh != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, kh.MaKh),
                        new Claim("FullName", kh.HoTen),
                        new Claim("Image", kh.Hinh)
                    };

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe,
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    HttpContext.Session.SetString("MaKh", kh.MaKh);
                    HttpContext.Session.SetString("HoTen", kh.HoTen);
                    HttpContext.Session.SetString("Hinh", kh.Hinh);

                    if (!string.IsNullOrEmpty(ReturnUrl))
                    {
                        return Redirect(ReturnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng");
            }
            return View(model);
        }
        #endregion

        #region DangXuat
        [HttpPost]
        public async Task<IActionResult> DangXuat()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("/");
        }
        #endregion
    }
}

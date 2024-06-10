using ECommerce.Data;
using ECommerce.Models;
using ECommerce.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using Microsoft.EntityFrameworkCore;

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
                if (db.KhachHangs.Any(k => k.MaKh == model.MaKh))
                {
                    ModelState.AddModelError("", "Tài khoản đã tồn tại. Vui lòng chọn tên đăng nhập khác.");
                    return View(model);
                }

                if (db.KhachHangs.Any(k => k.Email == model.Email))
                {
                    ModelState.AddModelError("", "Email đã được sử dụng. Vui lòng chọn email khác.");
                    return View(model);
                }

                try
                {
                    string hashedPassword = MD5Helper.GetMd5Hash(model.MatKhau);

                    string verificationCode = Guid.NewGuid().ToString();

                    SendVerificationEmail(model.Email, verificationCode);

                    var kh = new KhachHang
                    {
                        MaKh = model.MaKh,
                        MatKhau = hashedPassword,
                        Email = model.Email,
                        HieuLuc = false,
                        VaiTro = 0,
                        RandomKey = verificationCode,
                        HoTen = "Tên Mặc Định"
                    };
                    db.KhachHangs.Add(kh);
                    db.SaveChanges();

                    TempData["MaKh"] = model.MaKh;
                    return RedirectToAction("XacThucEmail");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                }
            }
            return View(model);
        }
        private void SendVerificationEmail(string email, string verificationCode)
        {
            var fromAddress = new MailAddress("ichigono2003@gmail.com", "DangNgocAnh");
            var toAddress = new MailAddress(email);
            const string fromPassword = "mwdz folf agqb xuzh";
            const string subject = "Xác thực tài khoản";
            string body = $"Mã xác thực của bạn là: {verificationCode}";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }
        }

        [HttpGet]
        public IActionResult XacThucEmail()
        {
            return View();
        }

        [HttpPost]
        public IActionResult XacThucEmail(string verificationCode)
        {
            try
            {
                string maKh = TempData["MaKh"]?.ToString();
                if (string.IsNullOrEmpty(maKh))
                {
                    ModelState.AddModelError("", "Không tìm thấy mã khách hàng");
                    return View();
                }

                // Bỏ qua phần kiểm tra mã xác thực và trực tiếp xác nhận
                var kh = db.KhachHangs.SingleOrDefault(k => k.MaKh == maKh);

                if (kh != null)
                {
                    kh.HieuLuc = true; // Đã xác thực
                    kh.RandomKey = null; // Xóa mã xác thực
                    db.SaveChanges();

                    return RedirectToAction("DangNhap");
                }

                // Trong trường hợp không tìm thấy khách hàng, cũng thông báo thành công
                return RedirectToAction("DangNhap");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
            }
            return View();
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
                string hashedPassword = MD5Helper.GetMd5Hash(model.Password);

                var kh = db.KhachHangs.SingleOrDefault(k => k.MaKh == model.Username && k.MatKhau == hashedPassword && k.HieuLuc);

                if (kh != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, kh.MaKh),
                        new Claim("FullName", kh.HoTen ?? ""),
                        new Claim(ClaimTypes.Role, kh.VaiTro == 1 ? "Admin" : "User"),
                        new Claim("Image", kh.Hinh ?? "")
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
                    HttpContext.Session.SetString("HoTen", kh.HoTen ?? "");
                    HttpContext.Session.SetString("Hinh", kh.Hinh ?? "");

                    if (!string.IsNullOrEmpty(ReturnUrl))
                    {
                        return Redirect(ReturnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng hoặc tài khoản chưa được xác thực");
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
            return RedirectToAction("Index", "Home");
        }
        #endregion

        #region ThongTinTaiKhoan
        [HttpGet]
        public IActionResult ThongTinTaiKhoan()
        {
            var maKh = HttpContext.Session.GetString("MaKh");
            if (string.IsNullOrEmpty(maKh))
            {
                return RedirectToAction("DangNhap");
            }

            var kh = db.KhachHangs.SingleOrDefault(k => k.MaKh == maKh);
            if (kh == null)
            {
                return NotFound();
            }

            var model = new ThongTinTaiKhoanViewModel
            {
                MaKh = kh.MaKh,
                Email = kh.Email,
                HoTen = kh.HoTen,
                GioiTinh = kh.GioiTinh,
                NgaySinh = kh.NgaySinh,
                DiaChi = kh.DiaChi,
                DienThoai = kh.DienThoai,
                Hinh = kh.Hinh
            };

            ViewData["UserImage"] = kh.Hinh;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThongTinTaiKhoan(ThongTinTaiKhoanViewModel model, IFormFile HinhFile)
        {
            if (ModelState.IsValid)
            {
                var maKh = HttpContext.Session.GetString("MaKh");
                if (string.IsNullOrEmpty(maKh))
                {
                    return RedirectToAction("DangNhap");
                }

                var kh = db.KhachHangs.SingleOrDefault(k => k.MaKh == maKh);
                if (kh == null)
                {
                    return NotFound();
                }

                if (HinhFile != null && HinhFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/KhachHang");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(HinhFile.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await HinhFile.CopyToAsync(fileStream);
                    }
                    kh.Hinh = $"/img/KhachHang/{uniqueFileName}";
                    model.Hinh = kh.Hinh;
                }

                kh.HoTen = model.HoTen;
                kh.GioiTinh = model.GioiTinh;
                kh.NgaySinh = (DateTime)model.NgaySinh;
                kh.DiaChi = model.DiaChi;
                kh.DienThoai = model.DienThoai;

                try
                {
                    db.KhachHangs.Update(kh);
                    await db.SaveChangesAsync();
                    ViewBag.Message = "Cập nhật thông tin thành công";
                    ViewData["UserImage"] = kh.Hinh;

                    return RedirectToAction("Index", "Home");
                }
                catch (DbUpdateException ex)
                {
                    Console.WriteLine($"Lỗi khi cập nhật thông tin: {ex.InnerException?.Message}");
                    ModelState.AddModelError("", $"Lỗi khi cập nhật thông tin: {ex.InnerException?.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi không xác định: {ex.Message}");
                    ModelState.AddModelError("", $"Lỗi không xác định: {ex.Message}");
                }
            }

            return View(model);
        }
        #endregion
    }
}


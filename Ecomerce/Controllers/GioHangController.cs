using ECommerce.Data;
using ECommerce.Helpers;
using ECommerce.Models;
using ECommerce.Services;
using ECommerce.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

public class GioHangController : Controller
{
    private readonly Hshop2023Context _context;
    private readonly IVnPayService _vnPayservice;

    public GioHangController(Hshop2023Context context, IVnPayService vnPayservice)
    {
        _context = context;
        _vnPayservice = vnPayservice;
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
        ViewBag.CartItemCount = GioHangItems.Sum(item => item.SoLuongMua);
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
                SoLuong = (int)hangHoa.SoLuong
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

    [Authorize]
    [HttpGet]
    public IActionResult ThanhToan()
    {
        if (GioHangItems.Count == 0)
        {
            return Redirect("/");
        }

        var model = new ThanhToanViewModel
        {
            GioHangItems = GioHangItems,
            PhiVanChuyen = 15
        };

        var user = _context.KhachHangs.SingleOrDefault(k => k.MaKh == User.Identity.Name);
        if (user != null)
        {
            model.HoTen = user.HoTen;
            model.DiaChi = user.DiaChi;
            model.DienThoai = user.DienThoai;
        }

        return View(model);
    }

    [Authorize]
    [HttpPost]
    public IActionResult ThanhToan(ThanhToanViewModel model, string payment = "COD")
    {
        model.GioHangItems = GioHangItems;

        if (ModelState.IsValid)
        {
            if (payment == "Thanh toán VNPay")
            {
                var vnPayModel = new VnPaymentRequestModel
                {
                    Amount = GioHangItems.Sum(p => p.ThanhTien),
                    CreatedDate = DateTime.Now,
                    Description = $"{model.HoTen} {model.DienThoai}",
                    FullName = model.HoTen,
                    OrderId = new Random().Next(1000, 100000)
                };
                return Redirect(_vnPayservice.CreatePaymentUrl(HttpContext, vnPayModel));
            }
            var hoadon = new HoaDon
            {
                MaKh = User.Identity.Name,
                NgayDat = DateTime.Now,
                HoTen = model.HoTen ?? "Tên Mặc Định",
                DiaChi = model.DiaChi ?? "Địa chỉ Mặc Định",
                DienThoai = model.DienThoai ?? "Số điện thoại Mặc Định",
                CachThanhToan = "Tien Mat",
                CachVanChuyen = "Ship",
                PhiVanChuyen = 15,
                MaTrangThai = 0,
                GhiChu = model.GhiChu ?? "Ghi chú Mặc Định"
            };

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    _context.HoaDons.Add(hoadon);
                    _context.SaveChanges();

                    Console.WriteLine("Hóa đơn mới được tạo với MaHd: " + hoadon.MaHd);

                    foreach (var item in GioHangItems)
                    {
                        var chitiet = new ChiTietHd
                        {
                            MaHd = hoadon.MaHd,
                            MaHh = item.MaHh,
                            SoLuong = item.SoLuongMua,
                            DonGia = (decimal)item.DonGia,
                            GiamGia = 0
                        };
                        _context.ChiTietHds.Add(chitiet);

                        Console.WriteLine($"Thêm chi tiết hóa đơn: MaHd={chitiet.MaHd}, MaHh={chitiet.MaHh}, SoLuong={chitiet.SoLuong}, DonGia={chitiet.DonGia}");

                        var hangHoa = _context.HangHoas.SingleOrDefault(hh => hh.MaHh == item.MaHh);
                        if (hangHoa != null)
                        {
                            hangHoa.SoLuong -= item.SoLuongMua;
                            _context.HangHoas.Update(hangHoa);

                            Console.WriteLine($"Cập nhật kho hàng: MaHh={hangHoa.MaHh}, SoLuong mới={hangHoa.SoLuong}");
                        }
                        else
                        {
                            throw new Exception("Không tìm thấy sản phẩm trong kho");
                        }
                    }

                    _context.SaveChanges();
                    transaction.Commit();

                    HttpContext.Session.Remove(MyConst.GIO_HANG);

                    return RedirectToAction("PaymentSuccess");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    TempData["Message"] = $"Đã xảy ra lỗi: {ex.Message}";

                    Console.WriteLine("Lỗi xảy ra: " + ex.Message);
                }
            }
        }
        else
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            ViewData["Errors"] = errors;
            TempData["Message"] = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại.";
        }

        return View(model);
    }

    [Authorize]
    public IActionResult PaymentCallBack()
    {
        var response = _vnPayservice.PaymentExecute(Request.Query);

        if (response == null || response.VnPayResponseCode != "00")
        {
            TempData["Message"] = $"Lỗi thanh toán VN Pay: {response.VnPayResponseCode}";
            return RedirectToAction("PaymentFail");
        }

        var hoadon = new HoaDon
        {
            MaKh = User.Identity.Name,
            NgayDat = DateTime.Now,
            HoTen = "Tên Mặc Định",
            DiaChi = "Địa chỉ Mặc Định",
            DienThoai = "Số điện thoại Mặc Định",
            CachThanhToan = "VNPay",
            CachVanChuyen = "Ship",
            PhiVanChuyen = 15,
            MaTrangThai = 1,
            GhiChu = "Ghi chú Mặc Định"
        };

        using (var transaction = _context.Database.BeginTransaction())
        {
            try
            {
                _context.HoaDons.Add(hoadon);
                _context.SaveChanges();

                foreach (var item in GioHangItems)
                {
                    var chitiet = new ChiTietHd
                    {
                        MaHd = hoadon.MaHd,
                        MaHh = item.MaHh,
                        SoLuong = item.SoLuongMua,
                        DonGia = (decimal)item.DonGia,
                        GiamGia = 0
                    };
                    _context.ChiTietHds.Add(chitiet);

                    var hangHoa = _context.HangHoas.SingleOrDefault(hh => hh.MaHh == item.MaHh);
                    if (hangHoa != null)
                    {
                        hangHoa.SoLuong -= item.SoLuongMua;
                        _context.HangHoas.Update(hangHoa);
                    }
                }

                _context.SaveChanges();
                transaction.Commit();

                HttpContext.Session.Remove(MyConst.GIO_HANG);

                TempData["Message"] = "Thanh toán VNPay thành công! Đơn hàng của bạn đã được gán trạng thái 'Đã thanh toán'.";
                return RedirectToAction("PaymentSuccess");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                TempData["Message"] = $"Đã xảy ra lỗi: {ex.Message}";
                return RedirectToAction("PaymentFail");
            }
        }
    }

    [Authorize]
    public async Task<IActionResult> DonHang(int page = 1)
    {
        var maKh = User.Identity.Name;
        var pageSize = 10;

        var ordersQuery = _context.HoaDons
            .Where(h => h.MaKh == maKh)
            .OrderBy(h => h.NgayDat)
            .Select(h => new DonHangViewModel
            {
                MaHd = h.MaHd,
                NgayDat = h.NgayDat,
                TongTien = h.ChiTietHds.Sum(ct => (double)ct.SoLuong * (double)ct.DonGia),
                TrangThai = h.MaTrangThaiNavigation.TenTrangThai
            });

        var totalOrders = await ordersQuery.CountAsync();
        var orders = await ordersQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var model = new DonHangListViewModel
        {
            DonHangs = orders,
            TotalOrders = totalOrders,
            CurrentPage = page,
            PageSize = pageSize
        };

        return View(model);
    }

    [Authorize]
    [HttpPost]
    public IActionResult DoiTrangThai(int id, int trangThai)
    {
        var hoadon = _context.HoaDons.Find(id);
        if (hoadon == null)
        {
            return NotFound();
        }

        hoadon.MaTrangThai = trangThai;
        _context.SaveChanges();

        return RedirectToAction("DonHang");
    }

    [Authorize]
    public IActionResult PaymentSuccess()
    {
        return View("ThanhToanThanhCong");
    }

    [Authorize]
    public IActionResult PaymentFail()
    {
        return View("ThanhToanThatBai");
    }
}

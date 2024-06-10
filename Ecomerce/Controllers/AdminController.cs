using ECommerce.Data;
using ECommerce.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly Hshop2023Context _context;

        public AdminController(Hshop2023Context context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            return View();
        }
        #region Users
        public async Task<IActionResult> Users(string searchField, string searchText, string statusFilter, int page = 1)
        {
            var query = _context.KhachHangs.AsQueryable();

            if (!string.IsNullOrEmpty(searchField) && !string.IsNullOrEmpty(searchText))
            {
                switch (searchField)
                {
                    case "HoTen":
                        query = query.Where(kh => kh.HoTen.Contains(searchText));
                        break;
                    case "Email":
                        query = query.Where(kh => kh.Email.Contains(searchText));
                        break;
                    case "DienThoai":
                        query = query.Where(kh => kh.DienThoai.Contains(searchText));
                        break;
                    case "DiaChi":
                        query = query.Where(kh => kh.DiaChi.Contains(searchText));
                        break;
                }
            }

            if (!string.IsNullOrEmpty(statusFilter))
            {
                var hieuLuc = statusFilter == "Đã xác thực";
                query = query.Where(kh => kh.HieuLuc == hieuLuc);
            }

            var totalUsers = await query.CountAsync();
            var users = await query.Skip((page - 1) * 10).Take(10).ToListAsync();

            var model = new UserListViewModel
            {
                Users = users,
                TotalUsers = totalUsers,
                SearchField = searchField,
                SearchText = searchText,
                StatusFilter = statusFilter,
                CurrentPage = page,
                PageSize = 10
            };

            return View(model);
        }
        #endregion

        public async Task<IActionResult> UserDetails(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.KhachHangs.FirstOrDefaultAsync(m => m.MaKh == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _context.KhachHangs.FindAsync(id);
            if (user != null)
            {
                _context.KhachHangs.Remove(user);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Users));
        }

        public IActionResult Orders(string searchField, string searchText, string dateFilter, int pageNumber = 1, int pageSize = 20)
        {
            var query = _context.HoaDons.AsQueryable();

            if (!string.IsNullOrEmpty(searchField) && !string.IsNullOrEmpty(searchText))
            {
                switch (searchField)
                {
                    case "HoTen":
                        query = query.Where(hd => hd.HoTen.Contains(searchText));
                        break;
                    case "Email":
                        query = query.Where(hd => hd.MaKhNavigation.Email.Contains(searchText));
                        break;
                    case "DienThoai":
                        query = query.Where(hd => hd.DienThoai.Contains(searchText));
                        break;
                    case "DiaChi":
                        query = query.Where(hd => hd.DiaChi.Contains(searchText));
                        break;
                }
            }

            if (DateTime.TryParse(dateFilter, out DateTime date))
            {
                query = query.Where(hd => hd.NgayDat.Date == date.Date);
            }

            var totalOrders = query.Count();
            var orders = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            var model = new OrderListViewModel
            {
                Orders = orders,
                TotalOrders = totalOrders,
                PageNumber = pageNumber,
                PageSize = pageSize,
                SearchField = searchField,
                SearchText = searchText,
                DateFilter = dateFilter
            };

            return View(model);
        }

        public IActionResult OrderDetails(int id)
        {
            var order = _context.HoaDons.FirstOrDefault(o => o.MaHd == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        public IActionResult DeleteOrder(int id)
        {
            var order = _context.HoaDons.FirstOrDefault(o => o.MaHd == id);
            if (order == null)
            {
                return NotFound();
            }

            _context.HoaDons.Remove(order);
            _context.SaveChanges();

            return RedirectToAction("Orders");
        }

    }
}

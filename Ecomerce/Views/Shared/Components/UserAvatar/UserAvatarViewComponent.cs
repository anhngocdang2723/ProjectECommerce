using Microsoft.AspNetCore.Mvc;
using ECommerce.Data;
using System.Threading.Tasks;

namespace ECommerce.ViewComponents
{
    public class UserAvatarViewComponent : ViewComponent
    {
        private readonly Hshop2023Context _context;

        public UserAvatarViewComponent(Hshop2023Context context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            string userImage = "~/img/KhachHang/OIP.jpg"; // Default user icon
            if (User.Identity.IsAuthenticated)
            {
                var maKh = HttpContext.Session.GetString("MaKh");
                if (!string.IsNullOrEmpty(maKh))
                {
                    var user = await _context.KhachHangs.FindAsync(maKh);
                    if (user != null && !string.IsNullOrEmpty(user.Hinh))
                    {
                        userImage = user.Hinh;
                    }
                }
            }

            return View("Default", userImage);
        }
    }
}

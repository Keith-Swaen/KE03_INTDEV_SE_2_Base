using Microsoft.AspNetCore.Mvc;

namespace KE03_INTDEV_SE_2_Base.Controllers
{
    public class OrdersController : Controller
    {
        public IActionResult OrderOverview()
        {
            return View();
        }
    }
}

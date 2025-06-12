// Controller: OrdersController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLayer;

namespace KE03_INTDEV_SE_2_Base.Controllers
{
    public class OrdersController : Controller
    {
        private readonly MatrixIncDbContext _context;

        public OrdersController(MatrixIncDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OrderOverview()
        {
            var orders = await _context.Orders
                .Include(o => o.Customer)
                .ToListAsync();

            return View(orders);
        }
    }
}


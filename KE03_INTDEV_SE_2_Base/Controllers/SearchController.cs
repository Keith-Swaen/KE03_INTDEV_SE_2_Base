using Microsoft.AspNetCore.Mvc;
using DataAccessLayer;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace KE03_INTDEV_SE_2_Base.Controllers
{
    public class SearchController : Controller
    {
        private readonly MatrixIncDbContext _context;
        public SearchController(MatrixIncDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Results(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                ViewBag.Query = q;
                return View();
            }
            var qLower = q.ToLower();
            List<DataAccessLayer.Models.Product> products = new();
            List<DataAccessLayer.Models.Customer> customers = new();
            List<DataAccessLayer.Models.Order> orders = new();

            if ("klant".Contains(qLower) || "klanten".Contains(qLower))
            {
                customers = await _context.Customers.ToListAsync();
            }
            else if ("product".Contains(qLower) || "producten".Contains(qLower))
            {
                products = await _context.Products.ToListAsync();
            }
            else if ("bestelling".Contains(qLower) || "bestellingen".Contains(qLower))
            {
                orders = await _context.Orders.Include(o => o.Customer).ToListAsync();
            }
            else
            {
                products = await _context.Products
                    .Where(p => p.Name.ToLower().Contains(qLower) || p.Description.ToLower().Contains(qLower))
                    .ToListAsync();
                customers = await _context.Customers
                    .Where(c => c.Name.ToLower().Contains(qLower) || c.Address.ToLower().Contains(qLower))
                    .ToListAsync();
                orders = await _context.Orders
                    .Include(o => o.Customer)
                    .Where(o => o.Id.ToString().Contains(qLower) || o.Customer.Name.ToLower().Contains(qLower))
                    .ToListAsync();
            }

            ViewBag.Query = q;
            ViewBag.Products = products;
            ViewBag.Customers = customers;
            ViewBag.Orders = orders;
            return View();
        }
    }
} 
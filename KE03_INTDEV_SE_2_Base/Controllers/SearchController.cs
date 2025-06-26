using Microsoft.AspNetCore.Mvc;
using DataAccessLayer;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace KE03_INTDEV_SE_2_Base.Controllers
{
    // Controller voor zoeken in producten, klanten en bestellingen
    // Hier kun je zoeken op verschillende entiteiten via de zoekbalk
    // Dependency injection wordt gebruikt voor de database context
    public class SearchController : Controller
    {
        private readonly MatrixIncDbContext _context;
        /*
         * Constructor van de controller
         * Hier wordt de dependency (de database context) via dependency injection binnengehaald
         */
        public SearchController(MatrixIncDbContext context)
        {
            _context = context;
        }

        /*
         * Deze actie toont de zoekresultatenpagina
         * async zodat de UI niet blokkeert bij het zoeken
         * Zoekt op producten, klanten en bestellingen op basis van de query
         */
        [HttpGet]
        public async Task<IActionResult> Results(string q)
        {
            // Controleren of de zoekopdracht leeg is
            if (string.IsNullOrWhiteSpace(q))
            {
                // Lege zoekopdracht, gewoon de view tonen
                ViewBag.Query = q;
                return View();
            }
            // Zoekterm omzetten naar kleine letters voor case-insensitive zoeken
            var qLower = q.ToLower();
            // Lijsten aanmaken voor de resultaten
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
                // Zoeken in producten op naam of beschrijving
                products = await _context.Products
                    .Where(p => p.Name.ToLower().Contains(qLower) || p.Description.ToLower().Contains(qLower))
                    .ToListAsync();
                // Zoeken in klanten op naam of adres
                customers = await _context.Customers
                    .Where(c => c.Name.ToLower().Contains(qLower) || c.Address.ToLower().Contains(qLower))
                    .ToListAsync();
                // Zoeken in bestellingen op id of klantnaam
                orders = await _context.Orders
                    .Include(o => o.Customer)
                    .Where(o => o.Id.ToString().Contains(qLower) || o.Customer.Name.ToLower().Contains(qLower))
                    .ToListAsync();
            }

            // Zoekterm meegeven aan de view
            ViewBag.Query = q;
            // Resultaten naar de view sturen
            ViewBag.Products = products;
            ViewBag.Customers = customers;
            ViewBag.Orders = orders;
            return View();
        }
    }
} 
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLayer;
using Microsoft.AspNetCore.Mvc.Rendering;
using DataAccessLayer.Models;
using KE03_INTDEV_SE_2_Base.Models;
using System.Text;
using System.Globalization;

namespace KE03_INTDEV_SE_2_Base.Controllers
{
    // Controller voor alles met bestellingen
    // Hier maak je bestellingen aan, bewerk je ze, verwijder je ze en kun je ze exporteren
    [Route("Bestellingen")]
    public class OrdersController : Controller
    {
        // Hier wordt de database context klaargezet zodat je overal in deze controller bij de database kunt
        private readonly MatrixIncDbContext _context;

        /*
         * Constructor van de controller
         * Hier wordt de dependency (de database context) via dependency injection binnengehaald
         */
        public OrdersController(MatrixIncDbContext context)
        {
            _context = context;
        }

        /*
         * Actie voor het tonen van het overzicht van alle bestellingen
         * async, zodat de UI niet blokkeert bij wachten op data
         * Task<IActionResult> deze methode een asynchrone actie is die een view of redirect teruggeeft
         */
        [Route("")]
        public async Task<IActionResult> Index()
        {
            // bestellingen ophalen met klant en producten
            var orders = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderProducts)
                .ToListAsync();
            // bestellingen naar de view sturen
            return View(orders);
        }

        /*
         * GET-actie voor het bewerken van een bestelling
         * Haalt de bestelling op en toont het bewerkformulier
         */
        [Route("Bewerken/{id}")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            // bestelling ophalen met klant en producten
            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
                .FirstOrDefaultAsync(o => o.Id == id);
            // als bestelling niet gevonden is, foutmelding
            if (order == null) return NotFound();
            // bestelling naar de view sturen
            return View(order);
        }

        /*
         * POST-actie voor het opslaan van een bewerkte bestelling
         * [HttpPost] geeft aan dat deze actie alleen reageert op POST-verzoeken
         * [ValidateAntiForgeryToken] beschermt tegen CSRF-aanvallen
         * [Bind] zorgt dat alleen de genoemde properties uit het formulier worden gebonden
         */
        [HttpPost]
        [Route("Bewerken/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Status")] Order order)
        {
            // checken of id klopt
            if (id != order.Id)
                return NotFound();
            // bestaande bestelling ophalen
            var existingOrder = await _context.Orders.FindAsync(id);
            if (existingOrder == null)
                return NotFound();
            // alleen status bijwerken
            existingOrder.Status = order.Status;
            try
            {
                // opslaan in database
                await _context.SaveChangesAsync();
                // terug naar overzicht
                return RedirectToAction(nameof(Index)); 
            }
            catch
            {
                // bij fout, foutmelding tonen
                ModelState.AddModelError("", "Er is iets misgegaan bij het opslaan.");
            }
            // bij fout, opnieuw ophalen en tonen
            existingOrder = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderProducts).ThenInclude(p => p.Product)
                .FirstOrDefaultAsync(o => o.Id == id);
            return View(existingOrder); 
        }

        /*
         * Actie voor het tonen van de details van een bestelling
         */
        [Route("Details/{id?}")]
        public async Task<IActionResult> Details(int? id)
        {
            // checken of id is meegegeven
            if (id == null)
            {
                return NotFound();
            }
            // bestelling ophalen met producten en klant
            var order = await _context.Orders
                .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.Product)
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.Id == id);
            if (order == null)
            {
                return NotFound();
            }
            // bestelling naar de view sturen
            return View(order);
        }

        /*
         * GET-actie voor het aanmaken van een nieuwe bestelling
         * Hier worden de dropdowns gevuld met klanten en producten
         */
        [Route("Aanmaken")]
        public async Task<IActionResult> Create()
        {
            // dropdowns vullen met klanten en producten
            var model = new OrderCreateViewModel
            {
                Customers = await _context.Customers
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    }).ToListAsync(),
                Products = await _context.Products
                    .Select(p => new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = p.Name
                    }).ToListAsync()
            };
            // model naar de view sturen
            return View(model);
        }

        /*
         * Hulpmethode om de dropdowns opnieuw te vullen bij validatiefouten
         */
        private async Task FillDropdownsAsync(OrderCreateViewModel model)
        {
            model.Customers = await _context.Customers
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                .ToListAsync();
            model.Products = await _context.Products
                .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Name })
                .ToListAsync();
        }

        /*
         * POST-actie voor het opslaan van een nieuwe bestelling
         * Hier wordt gecontroleerd of alles klopt en wordt de bestelling aangemaakt
         */
        [HttpPost]
        [Route("Aanmaken")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderCreateViewModel model)
        {
            // checken of klant is geselecteerd
            if (model.CustomerId == 0)
            {
                ModelState.AddModelError(nameof(model.CustomerId), "Selecteer een klant.");
            }
            // checken of er producten zijn toegevoegd
            if (model.OrderProducts == null || !model.OrderProducts.Any())
            {
                ModelState.AddModelError(string.Empty, "Voeg minstens een product toe aan de bestelling.");
            }
            // voorraad controleren per product
            foreach (var item in model.OrderProducts)
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == item.ProductId);
                if (product == null)
                {
                    ModelState.AddModelError(string.Empty, $"Product met ID {item.ProductId} bestaat niet (meer).");
                    continue;
                }
                if (item.Quantity > product.StockQuantity)
                {
                    ModelState.AddModelError(string.Empty, $"Niet genoeg voorraad voor '{product.Name}'. Beschikbaar: {product.StockQuantity}, gevraagd: {item.Quantity}.");
                }
            }
            // bij fout: dropdowns opnieuw vullen en formulier opnieuw tonen
            if (!ModelState.IsValid)
            {
                await FillDropdownsAsync(model);
                return View(model);
            }
            // bestelling aanmaken
            var order = new Order
            {
                CustomerId = model.CustomerId,
                OrderDate = model.OrderDate,
                Status = OrderStatus.InWacht,
                OrderProducts = new List<OrderProduct>()
            };
            // producten toevoegen aan bestelling
            foreach (var item in model.OrderProducts)
            {
                var product = await _context.Products.FirstAsync(p => p.Id == item.ProductId);
                order.OrderProducts.Add(new OrderProduct
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ProductPrice = product.Price,
                    Quantity = item.Quantity
                });
            }
            // bestelling opslaan in database
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            // terug naar overzicht
            return RedirectToAction(nameof(Index));
        }

        /*
         * GET-actie om alle bestellingen te exporteren naar CSV
         * Hieronder wordt stap voor stap uitgelegd wat er gebeurt:
         * - Eerst worden alle bestellingen opgehaald met klant en producten
         * - Er wordt een StringBuilder aangemaakt om de CSV op te bouwen
         * - De headerregel van de CSV wordt toegevoegd
         * - Voor elke bestelling wordt een regel toegevoegd met id, datum, klant, status, totaal en producten
         * - De naam van de klant wordt opgehaald, als die er niet is staat er 'Onbekend'
         * - De status en datum worden netjes geformatteerd
         * - Het totaalbedrag wordt berekend
         * - De producten worden als string samengevoegd
         * - Daarna wordt de bestandsnaam gemaakt met de datum en tijd
         * - De CSV wordt omgezet naar bytes
         * - Uiteindelijk wordt het bestand als download teruggegeven
         */
        [Route("Export")]
        public IActionResult ExportOrders()
        {
            try
            {
                var orders = _context.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.OrderProducts)
                        .ThenInclude(op => op.Product)
                    .ToList();
                var csv = new StringBuilder();
                csv.AppendLine("Order ID,Orderdatum,Klantnaam,Status,Totaalbedrag,Producten");
                foreach (var order in orders)
                {
                    var klantnaam = order.Customer?.Name ?? "Onbekend";
                    var status = order.Status.ToString();
                    var orderDatum = order.OrderDate.ToString("yyyy-MM-dd");
                    var totaal = order.OrderProducts.Sum(op => op.ProductPrice * op.Quantity).ToString("F2", new CultureInfo("nl-NL"));
                    var productInfo = string.Join(" | ", order.OrderProducts.Select(op =>
                    {
                        var naam = op.ProductName.Replace("\"", "\"\"");
                        return $"{naam} x{op.Quantity}";
                    }));
                    csv.AppendLine($"\"{order.Id}\",\"{orderDatum}\",\"{klantnaam}\",\"{status}\",\"{totaal}\",\"{productInfo}\"");
                }
                var fileName = $"bestellingen_export_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                var bytes = Encoding.UTF8.GetBytes(csv.ToString());
                return File(bytes, "text/csv", fileName);
            }
            catch (Exception)
            {
                return RedirectToAction(nameof(Index));
            }
        }

        /*
         * GET-actie voor het verwijderen van een bestelling (bevestigingspagina)
         */
        [Route("Verwijderen/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            // bestelling ophalen
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            // bestelling naar de view sturen
            return View(order);
        }

        /*
         * POST-actie voor het echt verwijderen van een bestelling
         * [ActionName("Delete")] zorgt dat deze methode DeleteConfirmed heet maar wel op Delete reageert
         */
        [HttpPost, ActionName("Delete")]
        [Route("Verwijderen/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // bestelling ophalen
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            // bestelling verwijderen
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            // terug naar overzicht
            return RedirectToAction(nameof(Index));
        }

        /*
         * GET-actie om productinfo bij een bestelling te tonen
         * id is het product, orderId is de bestelling
         */
        [Route("ProductInfo/{id}/{orderId}")]
        public async Task<IActionResult> ProductInfo(int? id, int? orderId)
        {
            // checken of id is meegegeven
            if (id == null)
            {
                return NotFound();
            }
            // product ophalen met categorie
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            // orderId meesturen naar de view
            ViewBag.OrderId = orderId;
            return View(product);
        }
    }
}


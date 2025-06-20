// Controller: OrdersController.cs
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
    public class OrdersController : Controller
    {
        private readonly MatrixIncDbContext _context;

        public OrdersController(MatrixIncDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderProducts)
                .ToListAsync();

            return View(orders);
        }
      

        // GET: Orders/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderProducts)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            return View(order);
        }

        // POST: Orders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Status")] Order order)
        {
            if (id != order.Id)
                return NotFound();

            var existingOrder = await _context.Orders.FindAsync(id);
            if (existingOrder == null)
                return NotFound();

            // Alleen status bijwerken
            existingOrder.Status = order.Status;

            try
            {
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index)); // ✅ redirect naar Index
            }
            catch
            {
                // Foutafhandeling
                ModelState.AddModelError("", "Er is iets misgegaan bij het opslaan.");
            }

            // Navigatieproperties zijn hier niet geladen, dus opnieuw ophalen
            existingOrder = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderProducts).ThenInclude(p => p.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            return View(existingOrder); // ✅ juiste data tonen
        }

        
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = _context.Orders
                .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.Product)
                .Include(o => o.Customer)
                .FirstOrDefault(o => o.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        public async Task<IActionResult> Create()
        {
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

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Als de view opnieuw gerenderd wordt (bij fouten), opnieuw dropdowns vullen
                model.Customers = await _context.Customers
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                    .ToListAsync();

                model.Products = await _context.Products
                    .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Name })
                    .ToListAsync();

                return View(model);
            }

            // Haal alle relevante producten op in één query
            var selectedProductIds = model.OrderProducts.Select(x => x.ProductId).ToList();
            var productDict = await _context.Products
                .Where(p => selectedProductIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id);

            // Validatie: controleer of genoeg voorraad beschikbaar is
            foreach (var item in model.OrderProducts)
            {
                if (!productDict.TryGetValue(item.ProductId, out var product))
                {
                    ModelState.AddModelError(string.Empty, $"Product met ID {item.ProductId} bestaat niet.");
                    return View(model); // Stop en toon fout
                }

                if (item.Quantity > product.StockQuantity)
                {
                    ModelState.AddModelError(string.Empty, $"Niet genoeg voorraad voor '{product.Name}'. Beschikbaar: {product.StockQuantity}, gevraagd: {item.Quantity}.");
                    return View(model); // Stop en toon fout
                }
            }

            // Bouw bestelling op
            var order = new Order
            {
                CustomerId = model.CustomerId,
                OrderDate = model.OrderDate,
                Status = OrderStatus.InWacht,
                OrderProducts = new List<OrderProduct>()
            };

            foreach (var item in model.OrderProducts)
            {
                var product = productDict[item.ProductId];

                // Verlaag de voorraad
                product.StockQuantity -= item.Quantity;

                // Voeg OrderProduct toe aan order
                order.OrderProducts.Add(new OrderProduct
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    ProductName = product.Name,
                    ProductPrice = product.Price
                });
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }



        //Products/Export
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
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Index));
            }
        }

    }

}


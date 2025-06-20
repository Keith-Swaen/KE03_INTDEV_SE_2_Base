// Controller: OrdersController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLayer;
using Microsoft.AspNetCore.Mvc.Rendering;
using DataAccessLayer.Models;

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
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var order = await _context.Orders.FindAsync(id);
        //    if (order == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(order);
        //}
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

            // ❌ Navigatieproperties zijn hier niet geladen, dus opnieuw ophalen
            existingOrder = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderProducts).ThenInclude(p => p.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            return View(existingOrder); // ✅ juiste data tonen
        }

        //public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Address,Active")] Customer customer)
        //{
        //    if (id != customer.Id)
        //    {
        //        _logger.LogWarning("Klant ID komt niet overeen bij bewerken: {Id} != {CustomerId}", id, customer.Id);
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _logger.LogInformation("Klant wordt bijgewerkt met ID: {CustomerId}", customer.Id);
        //            _context.Update(customer);
        //            await _context.SaveChangesAsync();
        //            _logger.LogInformation("Klant succesvol bijgewerkt met ID: {CustomerId}", customer.Id);
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!CustomerExists(customer.Id))
        //            {
        //                _logger.LogWarning("Klant niet gevonden tijdens gelijktijdige update met ID: {CustomerId}", customer.Id);
        //                return NotFound();
        //            }
        //            else
        //            {
        //                _logger.LogError("Gelijktijdigheidsfout bij het bijwerken van klant met ID: {CustomerId}", customer.Id);
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    _logger.LogWarning("Ongeldige modelstatus bij het bewerken van klant met ID: {CustomerId}", customer.Id);
        //    return View(customer);
        //}



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
    }
}


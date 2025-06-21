using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer;
using DataAccessLayer.Models;
using Microsoft.Extensions.Logging;

namespace KE03_INTDEV_SE_2_Base.Controllers
{
    public class CustomersController : Controller
    {
        private readonly MatrixIncDbContext _context;
        private readonly ILogger<CustomersController> _logger;

        public CustomersController(MatrixIncDbContext context, ILogger<CustomersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Klanten
        public async Task<IActionResult> Index(string filter = "all")
        {
            _logger.LogInformation("Klanten worden opgehaald, filter: {Filter}", filter);
            ViewBag.Filter = filter;

            IQueryable<Customer> query = _context.Customers;
            switch (filter?.ToLower())
            {
                case "active":
                    query = query.Where(c => c.Active);
                    break;
                case "inactive":
                    query = query.Where(c => !c.Active);
                    break;
                // Geen filter, toon alles
                default:
                    // No filter, show all
                    break;
            }

            var customers = await query.ToListAsync();
            return View(customers);
        }

        // GET: Klanten/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Klant details opgevraagd met null ID");
                return NotFound();
            }

            _logger.LogInformation("Klant details worden opgehaald voor ID: {CustomerId}", id);
            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                _logger.LogWarning("Klant niet gevonden met ID: {CustomerId}", id);
                return NotFound();
            }

            return View(customer);
        }

        // GET: Klanten/Create
        public IActionResult Create()
        {
            _logger.LogInformation("Klant aanmaakformulier wordt weergegeven");
            return View();
        }

        // POST: Klanten/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Address,Active")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                customer.Active = true; 
                _logger.LogInformation("Nieuwe klant wordt aangemaakt: {CustomerName}", customer.Name);
                _context.Add(customer);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Klant succesvol aangemaakt met ID: {CustomerId}", customer.Id);
                return RedirectToAction(nameof(Index));
            }
            _logger.LogWarning("Ongeldige modelstatus bij het aanmaken van klant");
            return View(customer);
        }

        // GET: Klanten/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Klant bewerken opgevraagd met null ID");
                return NotFound();
            }

            _logger.LogInformation("Klant wordt opgehaald voor bewerken met ID: {CustomerId}", id);
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                _logger.LogWarning("Klant niet gevonden voor bewerken met ID: {CustomerId}", id);
                return NotFound();
            }
            return View(customer);
        }

        // POST: Klanten/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Address,Active")] Customer customer)
        {
            if (id != customer.Id)
            {
                _logger.LogWarning("Klant ID komt niet overeen bij bewerken: {Id} != {CustomerId}", id, customer.Id);
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _logger.LogInformation("Klant wordt bijgewerkt met ID: {CustomerId}", customer.Id);
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Klant succesvol bijgewerkt met ID: {CustomerId}", customer.Id);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.Id))
                    {
                        _logger.LogWarning("Klant niet gevonden tijdens gelijktijdige update met ID: {CustomerId}", customer.Id);
                        return NotFound();
                    }
                    else
                    {
                        _logger.LogError("Gelijktijdigheidsfout bij het bijwerken van klant met ID: {CustomerId}", customer.Id);
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            _logger.LogWarning("Ongeldige modelstatus bij het bewerken van klant met ID: {CustomerId}", customer.Id);
            return View(customer);
        }

        // GET: Klanten/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Klant verwijderen opgevraagd met null ID");
                return NotFound();
            }

            _logger.LogInformation("Klant wordt opgehaald voor verwijderen met ID: {CustomerId}", id);
            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                _logger.LogWarning("Klant niet gevonden voor verwijderen met ID: {CustomerId}", id);
                return NotFound();
            }

            return View(customer);
        }

        // POST: Klanten/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _logger.LogInformation("Poging tot verwijderen van klant met ID: {CustomerId}", id);
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                customer.Active = false;
                _context.Update(customer);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Klant succesvol gedeactiveerd met ID: {CustomerId}", id);
            }
            else
            {
                _logger.LogWarning("Klant niet gevonden voor verwijderen met ID: {CustomerId}", id);
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }
    }
} 
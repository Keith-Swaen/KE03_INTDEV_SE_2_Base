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
    // Controller voor alles met klanten
    // Hier kun je klanten aanmaken, bewerken, verwijderen en details bekijken
    [Route("Klanten")]
    public class CustomersController : Controller
    {
        // database context en logger klaarzetten
        private readonly MatrixIncDbContext _context;
        private readonly ILogger<CustomersController> _logger;

        /*
         * Constructor van de controller
         * Hier worden de dependencies (database context en logger) via dependency injection binnengehaald
         */
        public CustomersController(MatrixIncDbContext context, ILogger<CustomersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /*
         * Actie voor het tonen van het overzicht van alle klanten
         * async, zodat de UI niet blokkeert bij wachten op data
         * Task<IActionResult> deze methode een asynchrone actie is die een view of redirect teruggeeft
         * Je kunt filteren op actief, inactief of alle klanten
         */
        [Route("")]
        [Route("{filter?}")]
        public async Task<IActionResult> Index(string filter = "alle")
        {
            _logger.LogInformation("Klanten worden opgehaald, filter: {Filter}", filter);
            ViewBag.Filter = filter;
            // query opbouwen op basis van filter
            IQueryable<Customer> query = _context.Customers;
            switch (filter?.ToLower())
            {
                case "actief":
                    query = query.Where(c => c.Active);
                    break;
                case "inactief":
                    query = query.Where(c => !c.Active);
                    break;
                default:
                    break;
            }
            // klanten ophalen en naar de view sturen
            var customers = await query.ToListAsync();
            return View(customers);
        }

        /*
         * Actie voor het tonen van de details van een klant
         * async, zodat de UI niet blokkeert bij wachten op data
         */
        [Route("Details/{id?}")]
        public async Task<IActionResult> Details(int? id)
        {
            // checken of id is meegegeven
            if (id == null)
            {
                _logger.LogWarning("Klant details opgevraagd met null ID");
                return NotFound();
            }
            // klant ophalen op id
            _logger.LogInformation("Klant details worden opgehaald voor ID: {CustomerId}", id);
            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.Id == id);
            // als klant niet bestaat, foutmelding
            if (customer == null)
            {
                _logger.LogWarning("Klant niet gevonden met ID: {CustomerId}", id);
                return NotFound();
            }
            // klant naar de view sturen
            return View(customer);
        }

        /*
         * GET-actie voor het aanmaken van een nieuwe klant
         * Toont het aanmaakformulier
         */
        [Route("Aanmaken")]
        public IActionResult Create()
        {
            _logger.LogInformation("Klant aanmaakformulier wordt weergegeven");
            return View();
        }

        /*
         * POST-actie voor het opslaan van een nieuwe klant
         * [Bind] zorgt dat alleen de genoemde properties uit het formulier worden gebonden
         */
        [HttpPost]
        [Route("Aanmaken")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Address,Active")] Customer customer)
        {
            // checken of model geldig is
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Nieuwe klant wordt aangemaakt: {CustomerName}", customer.Name);
                _context.Add(customer);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Klant succesvol aangemaakt met ID: {CustomerId}", customer.Id);
                // terug naar overzicht
                return RedirectToAction(nameof(Index));
            }
            _logger.LogWarning("Ongeldige modelstatus bij het aanmaken van klant");
            // bij fout opnieuw formulier tonen
            return View(customer);
        }

        /*
         * GET-actie voor het bewerken van een klant
         * Haalt de klant op en toont het bewerkformulier
         */
        [Route("Bewerken/{id?}")]
        public async Task<IActionResult> Edit(int? id)
        {
            // checken of id is meegegeven
            if (id == null)
            {
                _logger.LogWarning("Klant bewerken opgevraagd met null ID");
                return NotFound();
            }
            // klant ophalen
            _logger.LogInformation("Klant wordt opgehaald voor bewerken met ID: {CustomerId}", id);
            var customer = await _context.Customers.FindAsync(id);
            // als klant niet bestaat, foutmelding
            if (customer == null)
            {
                _logger.LogWarning("Klant niet gevonden voor bewerken met ID: {CustomerId}", id);
                return NotFound();
            }
            // klant naar de view sturen
            return View(customer);
        }

        /*
         * POST-actie voor het opslaan van een bewerkte klant
         * [HttpPost] geeft aan dat deze actie alleen reageert op POST-verzoeken
         * [ValidateAntiForgeryToken] beschermt tegen CSRF-aanvallen
         * [Bind] zorgt dat alleen de genoemde properties uit het formulier worden gebonden
         */
        [HttpPost]
        [Route("Bewerken/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Address,Active")] Customer customer)
        {
            // checken of id klopt
            if (id != customer.Id)
            {
                _logger.LogWarning("Klant ID komt niet overeen bij bewerken: {Id} != {CustomerId}", id, customer.Id);
                return NotFound();
            }
            // checken of model geldig is
            if (ModelState.IsValid)
            {
                try
                {
                    // klant bijwerken in database
                    _logger.LogInformation("Klant wordt bijgewerkt met ID: {CustomerId}", customer.Id);
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Klant succesvol bijgewerkt met ID: {CustomerId}", customer.Id);
                }
                catch (DbUpdateConcurrencyException)
                {
                    // als klant niet meer bestaat tijdens update
                    if (!CustomerExists(customer.Id))
                    {
                        _logger.LogWarning("Klant niet gevonden tijdens gelijktijdige update met ID: {CustomerId}", customer.Id);
                        return NotFound();
                    }
                    else
                    {
                        // als er iets anders misgaat
                        _logger.LogError("Gelijktijdigheidsfout bij het bijwerken van klant met ID: {CustomerId}", customer.Id);
                        throw;
                    }
                }
                // terug naar overzicht
                return RedirectToAction(nameof(Index));
            }
            _logger.LogWarning("Ongeldige modelstatus bij het bewerken van klant met ID: {CustomerId}", customer.Id);
            // bij fout opnieuw formulier tonen
            return View(customer);
        }

        /*
         * GET-actie voor het verwijderen van een klant (bevestigingspagina)
         * Haalt de klant op en toont de bevestiging
         */
        [Route("Verwijderen/{id?}")]
        public async Task<IActionResult> Delete(int? id)
        {
            // checken of id is meegegeven
            if (id == null)
            {
                _logger.LogWarning("Klant verwijderen opgevraagd met null ID");
                return NotFound();
            }
            // klant ophalen
            _logger.LogInformation("Klant wordt opgehaald voor verwijderen met ID: {CustomerId}", id);
            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.Id == id);
            // als klant niet bestaat, foutmelding
            if (customer == null)
            {
                _logger.LogWarning("Klant niet gevonden voor verwijderen met ID: {CustomerId}", id);
                return NotFound();
            }
            // klant naar de view sturen
            return View(customer);
        }

        /*
         * POST-actie voor het echt verwijderen (deactiveren) van een klant
         * [HttpPost, ActionName("Delete")] zorgt dat deze methode DeleteConfirmed heet maar wel op Delete reageert
         */
        [HttpPost, ActionName("Delete")]
        [Route("Verwijderen/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _logger.LogInformation("Poging tot verwijderen van klant met ID: {CustomerId}", id);
            // klant ophalen
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                // klant deactiveren (Active op false)
                customer.Active = false;
                _context.Update(customer);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Klant succesvol gedeactiveerd met ID: {CustomerId}", id);
            }
            else
            {
                _logger.LogWarning("Klant niet gevonden voor verwijderen met ID: {CustomerId}", id);
            }
            // terug naar overzicht
            return RedirectToAction(nameof(Index));
        }

        // Hulpmethode om te checken of een klant bestaat
        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }
    }
} 
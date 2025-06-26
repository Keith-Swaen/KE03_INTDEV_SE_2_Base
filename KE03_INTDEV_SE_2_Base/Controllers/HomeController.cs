using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using KE03_INTDEV_SE_2_Base.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DataAccessLayer;

namespace KE03_INTDEV_SE_2_Base.Controllers
{
    // Controller voor de homepagina, login, privacy en foutafhandeling
    // Hier worden het dashboard (home), login, privacy en errorpagina's getoond
    // Dependency injection wordt gebruikt voor de logger en database context
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MatrixIncDbContext _context;

        /*
         * Constructor van de controller
         * Hier worden de dependencies (logger en database context) via dependency injection binnengehaald
         */
        public HomeController(ILogger<HomeController> logger, MatrixIncDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        /*
         * Deze actie toont het dashboardoverzicht op de homepagina
         */
        [Authorize]
        public IActionResult Index()
        {
            // DashboardViewModel vullen met totalen
            var dashboard = new DashboardViewModel
            {
                // Totaal aantal producten ophalen
                TotalProducts = _context.Products.Count(),
                // Totaal aantal klanten ophalen
                TotalCustomers = _context.Customers.Count(),
                // Totaal aantal bestellingen ophalen
                TotalOrders = _context.Orders.Count()
            };

            // HomeViewModel vullen met dashboarddata
            var homeViewModel = new HomeViewModel
            {
                Dashboard = dashboard
            };

            // View tonen met het gevulde viewmodel
            return View(homeViewModel);
        }

        /*
         * Deze actie toont de privacy-pagina
         */
        public IActionResult Privacy()
        {
            // Simpelweg de privacy view tonen
            return View();
        }

        /*
         * GET-actie voor het tonen van het loginformulier
         */
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            // Als gebruiker al is ingelogd, direct doorsturen naar home
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            // Anders loginformulier tonen
            return View();
        }

        /*
         * POST-actie voor het verwerken van een loginpoging
         * async zodat de UI niet blokkeert bij wachten op authenticatie
         */
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            // Controleren of het model geldig is
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Login poging voor gebruiker: {Username}", model.Username);

                // Admin opzoeken in de database
                var admin = _context.Admins
                    .FirstOrDefault(a => a.Username == model.Username && a.Password == model.Password);

                if (admin != null)
                {
                    // Claims aanmaken voor de gebruiker
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, admin.Username)
                    };

                    // ClaimsIdentity aanmaken voor authenticatie
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    // Instellen dat de login persistent is
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                    };

                    // Inloggen uitvoeren
                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    _logger.LogInformation("Succesvolle login voor gebruiker: {Username}", admin.Username);
                    // Na succesvolle login doorsturen naar dashboard
                    return RedirectToAction("Index", "Dashboard");
                }

                _logger.LogWarning("Mislukte login poging voor gebruiker: {Username}", model.Username);
                ModelState.AddModelError("", "Ongeldige gebruikersnaam of wachtwoord");
            }
            else
            {
                _logger.LogWarning("Ongeldige modelstatus bij login poging");
            }

            // Bij fout opnieuw loginformulier tonen
            return View(model);
        }

        /*
         * POST-actie voor het uitloggen van de gebruiker
         * async zodat de UI niet blokkeert bij het afmelden
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            // Gebruikersnaam ophalen voor logging
            var username = User.Identity?.Name;
            // Uitloggen uitvoeren
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _logger.LogInformation("Gebruiker {Username} is uitgelogd", username);
            // Na uitloggen terug naar loginpagina
            return RedirectToAction("Login", "Home");
        }

        /*
         * Deze actie toont de errorpagina bij fouten
         */
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // RequestId ophalen voor foutopsporing
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            _logger.LogError("Er is een fout opgetreden. Request ID: {RequestId}", requestId);
            // ErrorViewModel vullen en tonen
            return View(new ErrorViewModel { RequestId = requestId });
        }
    }
}

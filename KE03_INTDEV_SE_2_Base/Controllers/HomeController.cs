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
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MatrixIncDbContext _context;

        public HomeController(ILogger<HomeController> logger, MatrixIncDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // [Authorize]
        // public IActionResult Index()
        // {
        //     return View();
        // }


        [Authorize]
        public IActionResult Index()
        {
            var dashboard = new DashboardViewModel
            {
                TotalProducts = _context.Products.Count(),
                TotalCustomers = _context.Customers.Count(),
                TotalOrders = _context.Orders.Count()
            };

            var homeViewModel = new HomeViewModel
            {
                Dashboard = dashboard
            };

            return View(homeViewModel);
        }



        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Login poging voor gebruiker: {Username}", model.Username);

                var admin = _context.Admins
                    .FirstOrDefault(a => a.Username == model.Username && a.Password == model.Password);

                if (admin != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, admin.Username)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    _logger.LogInformation("Succesvolle login voor gebruiker: {Username}", admin.Username);
                    return RedirectToAction("Index", "Dashboard");
                }

                _logger.LogWarning("Mislukte login poging voor gebruiker: {Username}", model.Username);
                ModelState.AddModelError("", "Ongeldige gebruikersnaam of wachtwoord");
            }
            else
            {
                _logger.LogWarning("Ongeldige modelstatus bij login poging");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var username = User.Identity?.Name;
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _logger.LogInformation("Gebruiker {Username} is uitgelogd", username);
            return RedirectToAction("Login", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            _logger.LogError("Er is een fout opgetreden. Request ID: {RequestId}", requestId);
            return View(new ErrorViewModel { RequestId = requestId });
        }
    }
}

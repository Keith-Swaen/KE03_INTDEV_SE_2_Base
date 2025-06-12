using Microsoft.AspNetCore.Mvc;
using DataAccessLayer.Models;
using System.Threading.Tasks;
using DataAccessLayer.Interfaces;
using System.Globalization;
using Microsoft.Extensions.Logging;

namespace KE03_INTDEV_SE_2_Base.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly CultureInfo _nlCulture = new CultureInfo("nl-NL");
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductRepository productRepository, ILogger<ProductsController> logger)
        {
            _productRepository = productRepository;
            _logger = logger;
        }

        // GET: Producten
        public IActionResult Index()
        {
            _logger.LogInformation("Alle producten worden opgehaald");
            var products = _productRepository.GetAllProducts();
            return View(products);
        }

        // GET: Producten/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Product details opgevraagd met null ID");
                return NotFound();
            }

            _logger.LogInformation("Product details worden opgehaald voor ID: {ProductId}", id);
            var product = _productRepository.GetProductById(id.Value);
            if (product == null)
            {
                _logger.LogWarning("Product niet gevonden met ID: {ProductId}", id);
                return NotFound();
            }

            return View(product);
        }

        // GET: Producten/Creëren
        public IActionResult Create()
        {
            _logger.LogInformation("Product aanmaakformulier wordt weergegeven");
            return View();
        }

        // POST: Producten/Creëren
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Name,Description,Price,StockQuantity")] Product product)
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Nieuw product wordt aangemaakt: {ProductName}", product.Name);
                _productRepository.AddProduct(product);
                _logger.LogInformation("Product succesvol aangemaakt met ID: {ProductId}", product.Id);
                return RedirectToAction(nameof(Index));
            }
            _logger.LogWarning("Ongeldige modelstatus bij het aanmaken van product");
            return View(product);
        }

        // GET: Producten/Bewerken/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Product bewerken opgevraagd met null ID");
                return NotFound();
            }

            _logger.LogInformation("Product wordt opgehaald voor bewerken met ID: {ProductId}", id);
            var product = _productRepository.GetProductById(id.Value);
            if (product == null)
            {
                _logger.LogWarning("Product niet gevonden voor bewerken met ID: {ProductId}", id);
                return NotFound();
            }

            return View(product);
        }

        // POST: Producten/Bewerken/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Name,Description,Price,StockQuantity")] Product product)
        {
            if (id != product.Id)
            {
                _logger.LogWarning("Product ID komt niet overeen bij bewerken: {Id} != {ProductId}", id, product.Id);
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Verwerk de prijs in Nederlands formaat
                var priceStr = Request.Form["Price"].ToString().Trim();
                if (decimal.TryParse(priceStr, NumberStyles.Any, _nlCulture, out decimal price))
                {
                    product.Price = price;
                    _logger.LogInformation("Prijs succesvol geconverteerd naar Nederlands formaat: {Price}", price);
                }
                else
                {
                    _logger.LogWarning("Kon prijs niet converteren naar Nederlands formaat: {PriceString}", priceStr);
                }

                _logger.LogInformation("Product wordt bijgewerkt met ID: {ProductId}", product.Id);
                _productRepository.UpdateProduct(product);
                _logger.LogInformation("Product succesvol bijgewerkt met ID: {ProductId}", product.Id);
                return RedirectToAction(nameof(Index));
            }
            _logger.LogWarning("Ongeldige modelstatus bij het bewerken van product met ID: {ProductId}", product.Id);
            return View(product);
        }

        // GET: Producten/Verwijderen/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Product verwijderen opgevraagd met null ID");
                return NotFound();
            }

            _logger.LogInformation("Product wordt opgehaald voor verwijderen met ID: {ProductId}", id);
            var product = _productRepository.GetProductById(id.Value);
            if (product == null)
            {
                _logger.LogWarning("Product niet gevonden voor verwijderen met ID: {ProductId}", id);
                return NotFound();
            }

            return View(product);
        }

        // POST: Producten/Verwijderen/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _logger.LogInformation("Poging tot verwijderen van product met ID: {ProductId}", id);
            var product = _productRepository.GetProductById(id);
            if (product != null)
            {
                _productRepository.DeleteProduct(product);
                _logger.LogInformation("Product succesvol verwijderd met ID: {ProductId}", id);
            }
            else
            {
                _logger.LogWarning("Product niet gevonden voor verwijderen met ID: {ProductId}", id);
            }
            return RedirectToAction(nameof(Index));
        }
    }
} 
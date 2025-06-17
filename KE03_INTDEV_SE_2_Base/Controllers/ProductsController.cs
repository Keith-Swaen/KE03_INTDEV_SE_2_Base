using Microsoft.AspNetCore.Mvc;
using DataAccessLayer.Models;
using System.Threading.Tasks;
using DataAccessLayer.Interfaces;
using System.Globalization;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;

namespace KE03_INTDEV_SE_2_Base.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly CultureInfo _nlCulture = new CultureInfo("nl-NL");
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            ILogger<ProductsController> logger)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
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
        public async Task<IActionResult> Create()
        {
            _logger.LogInformation("Product aanmaakformulier wordt weergegeven");
            ViewBag.Categories = new SelectList(await _categoryRepository.GetAllCategoriesAsync(), "Id", "Name");
            return View();
        }

        // POST: Producten/Creëren
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Name,Description,Price,StockQuantity,CategoryId")] Product product)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _logger.LogInformation("Nieuw product wordt aangemaakt: {ProductName}", product.Name);
                    _productRepository.AddProduct(product);
                    _logger.LogInformation("Product succesvol aangemaakt met ID: {ProductId}", product.Id);
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fout bij het aanmaken van product: {ProductName}", product.Name);
                ModelState.AddModelError("", "Er is een fout opgetreden bij het aanmaken van het product.");
            }
            return View(product);
        }

        // GET: Producten/Bewerken/5
        public async Task<IActionResult> Edit(int? id)
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

            ViewBag.Categories = new SelectList(await _categoryRepository.GetAllCategoriesAsync(), "Id", "Name", product.CategoryId);
            return View(product);
        }

        // POST: Producten/Bewerken/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Name,Description,Price,StockQuantity,CategoryId")] Product product)
        {
            if (id != product.Id)
            {
                _logger.LogWarning("Product ID komt niet overeen bij bewerken: {Id} != {ProductId}", id, product.Id);
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _logger.LogInformation("Product wordt bijgewerkt met ID: {ProductId}", product.Id);
                    _productRepository.UpdateProduct(product);
                    _logger.LogInformation("Product succesvol bijgewerkt met ID: {ProductId}", product.Id);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Fout bij het bijwerken van product: {ProductId}", id);
                    ModelState.AddModelError("", "Er is een fout opgetreden bij het bijwerken van het product.");
                }
            }
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
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fout bij het verwijderen van product: {ProductId}", id);
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Products/AddCategory
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCategory(int productId, int categoryId)
        {
            try
            {
                _logger.LogInformation("Categorie toevoegen aan product: {ProductId}, Categorie: {CategoryId}", productId, categoryId);
                var success = await _productRepository.AddCategoryToProductAsync(productId, categoryId);
                if (success)
                {
                    _logger.LogInformation("Categorie succesvol toegevoegd aan product: {ProductId}", productId);
                    return RedirectToAction(nameof(Edit), new { id = productId });
                }
                _logger.LogWarning("Categorie toevoegen mislukt voor product: {ProductId}", productId);
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fout bij het toevoegen van categorie aan product: {ProductId}", productId);
                return RedirectToAction(nameof(Edit), new { id = productId });
            }
        }
    }
} 
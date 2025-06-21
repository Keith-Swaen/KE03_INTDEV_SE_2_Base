using Microsoft.AspNetCore.Mvc;
using DataAccessLayer.Models;
using System.Threading.Tasks;
using DataAccessLayer.Interfaces;
using System.Globalization;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using System.Text;

namespace KE03_INTDEV_SE_2_Base.Controllers
{
    [Route("Producten")]
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

        [Route("")]
        public IActionResult Index()
        {
            _logger.LogInformation("Alle producten worden opgehaald");
            var products = _productRepository.GetAllProducts();
            return View(products);
        }

        [Route("Details/{id?}")]
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

        [Route("Aanmaken")]
        public async Task<IActionResult> Create()
        {
            _logger.LogInformation("Product aanmaakformulier wordt weergegeven");
            ViewBag.Categories = new SelectList(await _categoryRepository.GetActiveCategoriesAsync(), "Id", "Name");
            return View();
        }

        [HttpPost]
        [Route("Aanmaken")]
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

        [Route("Bewerken/{id?}")]
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

            ViewBag.Categories = new SelectList(await _categoryRepository.GetActiveCategoriesAsync(), "Id", "Name", product.CategoryId);
            return View(product);
        }

        [HttpPost]
        [Route("Bewerken/{id}")]
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

        [Route("Verwijderen/{id?}")]
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

        [HttpPost, ActionName("Delete")]
        [Route("Verwijderen/{id}")]
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

        // GET: Products/Export
        [Route("Export")]
        public IActionResult Export()
        {
            try
            {
                _logger.LogInformation("Producten export wordt gestart");
                var products = _productRepository.GetAllProducts();
                
                var csv = new StringBuilder();
                csv.AppendLine("Naam,Beschrijving,Prijs,Voorraad,Categorie");
                
                foreach (var product in products)
                {
                    var categoryName = product.Category?.Name ?? "Geen categorie";
                    var description = product.Description?.Replace("\"", "\"\"") ?? "";
                    csv.AppendLine($"\"{product.Name}\",\"{description}\",{product.Price.ToString("F2", _nlCulture)},{product.StockQuantity},\"{categoryName}\"");
                }
                
                var fileName = $"producten_export_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                var bytes = Encoding.UTF8.GetBytes(csv.ToString());
                
                _logger.LogInformation("Producten export succesvol voltooid: {FileName}", fileName);
                return File(bytes, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fout bij het exporteren van producten");
                return RedirectToAction(nameof(Index));
            }
        }
    }
} 
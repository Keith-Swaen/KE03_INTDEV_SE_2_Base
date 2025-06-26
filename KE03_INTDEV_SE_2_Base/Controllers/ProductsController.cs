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
    // Controller voor alles met producten
    // Hier maak je producten aan, bewerk je ze, verwijder je ze en kun je ze exporteren
    [Route("Producten")]
    public class ProductsController : Controller
    {
        // repositories en logger klaarzetten
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly CultureInfo _nlCulture = new CultureInfo("nl-NL");
        private readonly ILogger<ProductsController> _logger;

        /*
         * Dit is de constructor, hiermee geef je de dependencies door aan deze controller
         */
        public ProductsController(
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            ILogger<ProductsController> logger)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _logger = logger;
        }

        /*
         * Deze methode laat het overzicht van alle producten zien
         * IActionResult betekent dat je een view of een redirect teruggeeft
         */
        [Route("")]
        public IActionResult Index()
        {
            // loggen dat producten worden opgehaald
            _logger.LogInformation("Alle producten worden opgehaald");
            // alle producten uit de repository halen
            var products = _productRepository.GetAllProducts();
            // producten naar de view sturen
            return View(products);
        }

        /*
         * Details laat de details van een product zien
         * int? betekent dat het id ook null mag zijn
         */
        [Route("Details/{id?}")]
        public IActionResult Details(int? id)
        {
            // checken of id is meegegeven
            if (id == null)
            {
                _logger.LogWarning("Product details opgevraagd met null ID");
                return NotFound();
            }
            // product ophalen op id
            _logger.LogInformation("Product details worden opgehaald voor ID: {ProductId}", id);
            var product = _productRepository.GetProductById(id.Value);
            // als product niet bestaat, foutmelding
            if (product == null)
            {
                _logger.LogWarning("Product niet gevonden met ID: {ProductId}", id);
                return NotFound();
            }
            // product naar de view sturen
            return View(product);
        }

        /*
         * Dit is de GET voor het aanmaken van een product
         * async betekent dat deze methode niet alles blokkeert, handig als je wacht op data
         */
        [Route("Aanmaken")]
        public async Task<IActionResult> Create()
        {
            // loggen dat formulier wordt getoond
            _logger.LogInformation("Product aanmaakformulier wordt weergegeven");
            // categorieen ophalen voor dropdown
            ViewBag.Categories = new SelectList(await _categoryRepository.GetActiveCategoriesAsync(), "Id", "Name");
            // view tonen
            return View();
        }

        /*
         * Dit is de POST voor het aanmaken van een product
         * [HttpPost] betekent dat deze alleen reageert op een POST
         * [ValidateAntiForgeryToken] is voor veiligheid
         * [Bind(...)] zorgt dat alleen deze velden uit het formulier komen
         */
        [HttpPost]
        [Route("Aanmaken")]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Name,Description,Price,StockQuantity,CategoryId")] Product product)
        {
            try
            {
                // checken of model geldig is
                if (ModelState.IsValid)
                {
                    // product toevoegen aan database
                    _logger.LogInformation("Nieuw product wordt aangemaakt: {ProductName}", product.Name);
                    _productRepository.AddProduct(product);
                    _logger.LogInformation("Product succesvol aangemaakt met ID: {ProductId}", product.Id);
                    // terug naar overzicht
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                // als er iets misgaat, foutmelding tonen
                _logger.LogError(ex, "Fout bij het aanmaken van product: {ProductName}", product.Name);
                ModelState.AddModelError("", "Er is een fout opgetreden bij het aanmaken van het product.");
            }
            // bij fout opnieuw formulier tonen
            return View(product);
        }

        /*
         * Dit is de GET voor het bewerken van een product
         * Haalt het product op en laat het bewerkformulier zien
         */
        [Route("Bewerken/{id?}")]
        public async Task<IActionResult> Edit(int? id)
        {
            // checken of id is meegegeven
            if (id == null)
            {
                _logger.LogWarning("Product bewerken opgevraagd met null ID");
                return NotFound();
            }
            // product ophalen
            _logger.LogInformation("Product wordt opgehaald voor bewerken met ID: {ProductId}", id);
            var product = _productRepository.GetProductById(id.Value);
            // als product niet bestaat, foutmelding
            if (product == null)
            {
                _logger.LogWarning("Product niet gevonden voor bewerken met ID: {ProductId}", id);
                return NotFound();
            }
            // categorieen ophalen voor dropdown
            ViewBag.Categories = new SelectList(await _categoryRepository.GetActiveCategoriesAsync(), "Id", "Name", product.CategoryId);
            // product naar de view sturen
            return View(product);
        }

        /*
         * Dit is de POST voor het opslaan van een bewerkt product
         * Controleert of het id klopt en valideert het model
         */
        [HttpPost]
        [Route("Bewerken/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Name,Description,Price,StockQuantity,CategoryId")] Product product)
        {
            // checken of id klopt
            if (id != product.Id)
            {
                _logger.LogWarning("Product ID komt niet overeen bij bewerken: {Id} != {ProductId}", id, product.Id);
                return NotFound();
            }
            // checken of model geldig is
            if (ModelState.IsValid)
            {
                try
                {
                    // product bijwerken in database
                    _logger.LogInformation("Product wordt bijgewerkt met ID: {ProductId}", product.Id);
                    _productRepository.UpdateProduct(product);
                    _logger.LogInformation("Product succesvol bijgewerkt met ID: {ProductId}", product.Id);
                    // terug naar overzicht
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // als er iets misgaat, foutmelding tonen
                    _logger.LogError(ex, "Fout bij het bijwerken van product: {ProductId}", id);
                    ModelState.AddModelError("", "Er is een fout opgetreden bij het bijwerken van het product.");
                }
            }
            // bij fout opnieuw formulier tonen
            return View(product);
        }

        /*
         * Dit is de GET voor het verwijderen van een product
         * Laat een bevestigingspagina zien
         */
        [Route("Verwijderen/{id?}")]
        public IActionResult Delete(int? id)
        {
            // checken of id is meegegeven
            if (id == null)
            {
                _logger.LogWarning("Product verwijderen opgevraagd met null ID");
                return NotFound();
            }
            // product ophalen
            _logger.LogInformation("Product wordt opgehaald voor verwijderen met ID: {ProductId}", id);
            var product = _productRepository.GetProductById(id.Value);
            // als product niet bestaat, foutmelding
            if (product == null)
            {
                _logger.LogWarning("Product niet gevonden voor verwijderen met ID: {ProductId}", id);
                return NotFound();
            }
            // product naar de view sturen
            return View(product);
        }

        /*
         * Dit is de POST voor het echt verwijderen van een product
         * [ActionName("Delete")] zorgt dat deze methode DeleteConfirmed heet maar wel op Delete reageert
         */
        [HttpPost, ActionName("Delete")]
        [Route("Verwijderen/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                // product ophalen
                _logger.LogInformation("Poging tot verwijderen van product met ID: {ProductId}", id);
                var product = _productRepository.GetProductById(id);
                // als product bestaat, verwijderen
                if (product != null)
                {
                    _productRepository.DeleteProduct(product);
                    _logger.LogInformation("Product succesvol verwijderd met ID: {ProductId}", id);
                }
                else
                {
                    _logger.LogWarning("Product niet gevonden voor verwijderen met ID: {ProductId}", id);
                }
                // terug naar overzicht
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // als er iets misgaat, terug naar overzicht
                _logger.LogError(ex, "Fout bij het verwijderen van product: {ProductId}", id);
                return RedirectToAction(nameof(Index));
            }
        }

        /*
         * Dit is de POST om een categorie toe te voegen aan een product
         * Je geeft het productId en categoryId mee
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCategory(int productId, int categoryId)
        {
            try
            {
                // loggen dat categorie wordt toegevoegd
                _logger.LogInformation("Categorie toevoegen aan product: {ProductId}, Categorie: {CategoryId}", productId, categoryId);
                // categorie toevoegen via repository
                var success = await _productRepository.AddCategoryToProductAsync(productId, categoryId);
                // als gelukt, terug naar bewerken
                if (success)
                {
                    _logger.LogInformation("Categorie succesvol toegevoegd aan product: {ProductId}", productId);
                    return RedirectToAction(nameof(Edit), new { id = productId });
                }
                // als niet gelukt, foutmelding
                _logger.LogWarning("Categorie toevoegen mislukt voor product: {ProductId}", productId);
                return NotFound();
            }
            catch (Exception ex)
            {
                // als er iets misgaat, terug naar bewerken
                _logger.LogError(ex, "Fout bij het toevoegen van categorie aan product: {ProductId}", productId);
                return RedirectToAction(nameof(Edit), new { id = productId });
            }
        }

        /*
         * Dit is de GET om alle producten te exporteren naar CSV
         * Je krijgt een download met alle producten
         * Hieronder wordt stap voor stap uitgelegd wat er gebeurt:
         * - Eerst wordt een logregel geschreven dat de export start
         * - Alle producten worden opgehaald uit de repository
         * - Er wordt een StringBuilder aangemaakt om de CSV op te bouwen
         * - De headerregel van de CSV wordt toegevoegd
         * - Voor elk product wordt een regel toegevoegd met naam, beschrijving, prijs, voorraad en categorie
         * - De naam van de categorie wordt opgehaald, als die er niet is staat er 'Geen categorie'
         * - De beschrijving wordt opgeschoond voor quotes
         * - De prijs wordt altijd met 2 decimalen en komma als decimaalteken toegevoegd
         * - Daarna wordt de bestandsnaam gemaakt met de datum en tijd
         * - De CSV wordt omgezet naar bytes
         * - Er wordt een logregel geschreven dat de export gelukt is
         * - Uiteindelijk wordt het bestand als download teruggegeven
         */
        [Route("Export")]
        public IActionResult Export()
        {
            try
            {
                // loggen dat export start
                _logger.LogInformation("Producten export wordt gestart");
                // alle producten ophalen
                var products = _productRepository.GetAllProducts();
                // StringBuilder voor CSV aanmaken
                var csv = new StringBuilder();
                // header toevoegen
                csv.AppendLine("Naam,Beschrijving,Prijs,Voorraad,Categorie");
                // voor elk product een regel toevoegen
                foreach (var product in products)
                {
                    // categorie ophalen, als die er niet is 'Geen categorie'
                    var categoryName = product.Category?.Name ?? "Geen categorie";
                    // beschrijving opschonen voor quotes
                    var description = product.Description?.Replace("\"", "\"\"") ?? "";
                    // regel toevoegen aan CSV
                    csv.AppendLine($"\"{product.Name}\",\"{description}\",{product.Price.ToString("F2", _nlCulture)},{product.StockQuantity},\"{categoryName}\"");
                }
                // bestandsnaam maken met datum en tijd
                var fileName = $"producten_export_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                // CSV omzetten naar bytes
                var bytes = Encoding.UTF8.GetBytes(csv.ToString());
                // loggen dat export gelukt is
                _logger.LogInformation("Producten export succesvol voltooid: {FileName}", fileName);
                // bestand als download teruggeven
                return File(bytes, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                // als er iets misgaat, terug naar overzicht
                _logger.LogError(ex, "Fout bij het exporteren van producten");
                return RedirectToAction(nameof(Index));
            }
        }
    }
} 
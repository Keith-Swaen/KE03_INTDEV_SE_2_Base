using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using System.Collections.Generic;

namespace KE03_INTDEV_SE_2_Base.Controllers
{
    // Controller voor alles met categorieën
    // Hier kun je categorieën aanmaken, bewerken, verwijderen en bekijken
    // Dependency injection wordt gebruikt voor de database context en logger
    [Route("Categorieen")]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;

        /*
         * Constructor van de controller
         * Hier wordt de dependency (de category repository) via dependency injection binnengehaald
         */
        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [Route("")]
        [Route("{filter?}")]
        /*
         * Actie voor het tonen van het overzicht van alle categorieën
         * async, zodat de UI niet blokkeert bij wachten op data
         * Je kunt filteren op actief, inactief of alle categorieën
         */
        public async Task<IActionResult> Index(string filter = "actief")
        {
            // Afhankelijk van de filter de juiste categorieën ophalen
            IEnumerable<Category> categories;
            if (filter == "alle")
            {
                // Alle categorieën ophalen
                categories = await _categoryRepository.GetAllCategoriesAsync();
            }
            else if (filter == "inactief")
            {
                // Alleen inactieve categorieën ophalen
                categories = await _categoryRepository.GetInactiveCategoriesAsync();
            }
            else
            {
                // Alleen actieve categorieën ophalen
                categories = await _categoryRepository.GetActiveCategoriesAsync();
            }
            // Filter meegeven aan de view
            ViewBag.Filter = filter;
            // Categorieën naar de view sturen
            return View(categories);
        }

        [Route("Aanmaken")]
        /*
         * GET-actie voor het aanmaken van een nieuwe categorie
         * Toont het aanmaakformulier
         */
        public IActionResult Create()
        {
            // Het lege formulier tonen
            return View();
        }

        [HttpPost]
        [Route("Aanmaken")]
        [ValidateAntiForgeryToken]
        /*
         * POST-actie voor het opslaan van een nieuwe categorie
         * [Bind] zorgt dat alleen de genoemde properties uit het formulier worden gebonden
         */
        public async Task<IActionResult> Create(Category category)
        {
            // Controleren of het model geldig is
            if (ModelState.IsValid)
            {
                // Nieuwe categorie aanmaken via de repository
                await _categoryRepository.CreateCategoryAsync(category);
                // Na opslaan terug naar het overzicht
                return RedirectToAction(nameof(Index));
            }
            // Bij fout opnieuw formulier tonen
            return View(category);
        }

        [Route("Bewerken/{id}")]
        /*
         * GET-actie voor het bewerken van een categorie
         * Haalt de categorie op en toont het bewerkformulier
         */
        public async Task<IActionResult> Edit(int id)
        {
            // Categorie ophalen op id
            var category = await _categoryRepository.GetCategoryByIdAsync(id);
            if (category == null)
            {
                // Als de categorie niet bestaat, foutmelding
                return NotFound();
            }
            // Categorie naar de view sturen
            return View(category);
        }

        [HttpPost]
        [Route("Bewerken/{id}")]
        [ValidateAntiForgeryToken]
        /*
         * POST-actie voor het opslaan van een bewerkte categorie
         * [Bind] zorgt dat alleen de genoemde properties uit het formulier worden gebonden
         */
        public async Task<IActionResult> Edit(int id, Category category)
        {
            // Controleren of het id overeenkomt
            if (id != category.Id)
            {
                return NotFound();
            }
            // Controleren of het model geldig is
            if (ModelState.IsValid)
            {
                // Categorie bijwerken via de repository
                await _categoryRepository.UpdateCategoryAsync(category);
                // Na opslaan terug naar het overzicht
                return RedirectToAction(nameof(Index));
            }
            // Bij fout opnieuw formulier tonen
            return View(category);
        }

        [Route("Verwijderen/{id}")]
        [HttpGet]
        /*
         * GET-actie voor het verwijderen van een categorie (bevestigingspagina)
         * Haalt de categorie op en toont de bevestiging
         */
        public async Task<IActionResult> Delete(int id)
        {
            // Categorie ophalen op id
            var category = await _categoryRepository.GetCategoryByIdAsync(id);
            if (category == null)
            {
                // Als de categorie niet bestaat, foutmelding
                return NotFound();
            }
            // Categorie naar de view sturen
            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [Route("Verwijderen/{id}")]
        [ValidateAntiForgeryToken]
        /*
         * POST-actie voor het echt verwijderen (deactiveren) van een categorie
         * [HttpPost, ActionName("Delete")] zorgt dat deze methode DeleteConfirmed heet maar wel op Delete reageert
         */
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Proberen de categorie te deactiveren
            var success = await _categoryRepository.DeactivateCategoryAsync(id); 
            if (!success)
            {
                // Als de categorie niet bestaat of niet gedeactiveerd kan worden
                return NotFound();
            }
            // Na verwijderen terug naar het overzicht
            return RedirectToAction(nameof(Index));
        }
    }
}

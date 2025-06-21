using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using System.Collections.Generic;

namespace KE03_INTDEV_SE_2_Base.Controllers
{
    [Route("Categorieen")]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [Route("")]
        [Route("{filter?}")]
        public async Task<IActionResult> Index(string filter = "actief")
        {
            IEnumerable<Category> categories;
            if (filter == "alle")
            {
                categories = await _categoryRepository.GetAllCategoriesAsync();
            }
            else if (filter == "inactief")
            {
                categories = await _categoryRepository.GetInactiveCategoriesAsync();
            }
            else
            {
                categories = await _categoryRepository.GetActiveCategoriesAsync();
            }
            ViewBag.Filter = filter;
            return View(categories);
        }

        [Route("Aanmaken")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Route("Aanmaken")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                category.Active = true;
                await _categoryRepository.CreateCategoryAsync(category);
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        [Route("Bewerken/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        [Route("Bewerken/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _categoryRepository.UpdateCategoryAsync(category);
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        [Route("Verwijderen/{id}")]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [Route("Verwijderen/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _categoryRepository.DeactivateCategoryAsync(id); 
            if (!success)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

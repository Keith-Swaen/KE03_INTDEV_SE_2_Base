using Microsoft.AspNetCore.Mvc;
using DataAccessLayer.Models;
using System.Threading.Tasks;
using DataAccessLayer.Interfaces;

namespace KE03_INTDEV_SE_2_Base.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductRepository _productRepository;

        public ProductsController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        // GET: Producten
        public IActionResult Index()
        {
            var products = _productRepository.GetAllProducts();
            return View(products);
        }

        // GET: Producten/creëren
        public IActionResult Create()
        {
            return View();
        }

        // POST: Producten/creëren 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Name,Description,Price")] Product product)
        {
            if (ModelState.IsValid)
            {
                _productRepository.AddProduct(product);
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }
    }
} 
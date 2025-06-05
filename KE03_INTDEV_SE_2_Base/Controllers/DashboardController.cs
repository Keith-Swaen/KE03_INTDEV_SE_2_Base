using DataAccessLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using KE03_INTDEV_SE_2_Base.Models;

namespace KE03_INTDEV_SE_2_Base.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IOrderRepository _orderRepository;

        public DashboardController(
            IProductRepository productRepository,
            ICustomerRepository customerRepository,
            IOrderRepository orderRepository)
        {
            _productRepository = productRepository;
            _customerRepository = customerRepository;
            _orderRepository = orderRepository;
        }

        public IActionResult Index()
        {
            var dashboardViewModel = new DashboardViewModel
            {
                TotalProducts = _productRepository.GetAllProducts().Count(),
                TotalCustomers = _customerRepository.GetAllCustomers().Count(),
                TotalOrders = _orderRepository.GetAllOrders().Count(),
                RecentOrders = _orderRepository.GetAllOrders()
                    .OrderByDescending(o => o.OrderDate)
                    .Take(5)
                    .ToList(),
                ProductsByCategory = _productRepository.GetAllProducts()
                    .GroupBy(p => p.Category ?? "Uncategorized")
                    .Select(g => new CategoryCount { Category = g.Key, Count = g.Count() })
                    .ToList()
            };

            return View(dashboardViewModel);
        }
    }
} 
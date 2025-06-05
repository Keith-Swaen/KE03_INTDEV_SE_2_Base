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
            var allOrders = _orderRepository.GetAllOrders().ToList();
            var allProducts = _productRepository.GetAllProducts().ToList();

            // Calculate average order value based on product prices
            var averageOrderValue = allProducts.Any() 
                ? allProducts.Average(p => p.Price) 
                : 0;

            // Get orders timeline for the last 7 days
            var last7Days = Enumerable.Range(0, 7)
                .Select(i => DateTime.Today.AddDays(-i))
                .Reverse()
                .ToList();

            var ordersTimeline = last7Days
                .Select(date => new TimelineData
                {
                    Date = date,
                    Count = allOrders.Count(o => o.OrderDate.Date == date)
                })
                .ToList();

            // Get top 5 products by price
            var topProducts = allProducts
                .OrderByDescending(p => p.Price)
                .Take(5)
                .Select(p => new ProductData
                {
                    Name = p.Name,
                    Price = p.Price
                })
                .ToList();

            // Group products by category
            var productsByCategory = allProducts
                .GroupBy(p => !string.IsNullOrEmpty(p.Category) ? p.Category : "Uncategorized")
                .Select(g => new CategoryCount 
                { 
                    Category = g.Key ?? "Uncategorized", 
                    Count = g.Count() 
                })
                .ToList();

            var dashboardViewModel = new DashboardViewModel
            {
                TotalProducts = allProducts.Count,
                TotalCustomers = _customerRepository.GetAllCustomers().Count(),
                TotalOrders = allOrders.Count,
                AverageOrderValue = averageOrderValue,
                RecentOrders = allOrders
                    .OrderByDescending(o => o.OrderDate)
                    .Take(5)
                    .ToList(),
                ProductsByCategory = productsByCategory,
                OrdersTimeline = ordersTimeline,
                TopProducts = topProducts
            };

            return View(dashboardViewModel);
        }
    }
} 
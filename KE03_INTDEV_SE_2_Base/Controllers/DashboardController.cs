using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using DataAccessLayer;
using KE03_INTDEV_SE_2_Base.Models;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer.Models;

namespace KE03_INTDEV_SE_2_Base.Controllers
{
    public class DashboardController : Controller
    {
        private readonly MatrixIncDbContext _context;

        public DashboardController(MatrixIncDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var ordersWithProducts = _context.Orders
                .Include(o => o.OrderProducts)
                .ToList();

            var viewModel = new DashboardViewModel
            {
                TotalProducts = _context.Products.Count(),
                TotalCustomers = _context.Customers.Where(c => c.Active).Count(),
                TotalOrders = ordersWithProducts.Count,
                TotalRevenue = ordersWithProducts
                    .SelectMany(o => o.OrderProducts)
                    .Sum(p => p.ProductPrice)
            };

            var priceRanges = new[] { 0m, 100m, 500m, 1000m, 5000m, decimal.MaxValue };
            var products = _context.Products.ToList(); 
            var distribution = new List<ChartDataPoint>();
            
            for (int i = 0; i < priceRanges.Length - 1; i++)
            {
                var currentMin = priceRanges[i];
                var currentMax = priceRanges[i + 1];
                var count = products.Count(p => 
                    p.Price >= currentMin && p.Price < currentMax);
                var label = $"€{currentMin} - {(currentMax == decimal.MaxValue ? "+" : "€" + currentMax.ToString())}";
                distribution.Add(new ChartDataPoint { Label = label, Value = count });
            }
            viewModel.ProductPriceDistribution = distribution;

            var last7Days = Enumerable.Range(0, 7)
                .Select(i => DateTime.Now.Date.AddDays(-i))
                .Reverse()
                .ToList();

            viewModel.OrdersOverTime = last7Days.Select(date => new ChartDataPoint
            {
                Label = date.ToString("MM/dd"),
                Value = ordersWithProducts.Count(o => o.OrderDate.Date == date)
            }).ToList();

           
            // werkt maar kost veel moeite bij grote dataset 
            var productsWithOrderProducts = _context.OrderProducts
                .Include(op => op.Product)
                .ToList(); // ⬅ haalt eerst alles op van de database

            viewModel.TopProducts = productsWithOrderProducts
                .GroupBy(op => op.Product)
                .Select(g => new TopProductViewModel
                {
                    ProductName = g.Key.Name,
                    OrderCount = g.Count(),
                    TotalRevenue = g.Sum(op => op.ProductPrice * op.Quantity) 
                })
                .OrderByDescending(p => p.OrderCount)
                .Take(5)
                .ToList();

            return View(viewModel);
        }
    }
} 

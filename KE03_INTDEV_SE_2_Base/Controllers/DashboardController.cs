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
    // Controller voor het dashboard
    // Hier worden de belangrijkste statistieken en overzichten getoond
    // Dependency injection wordt gebruikt voor de database context
    public class DashboardController : Controller
    {
        private readonly MatrixIncDbContext _context;

        /*
         * Constructor van de controller
         * Hier wordt de dependency (de database context) via dependency injection binnengehaald
         */
        public DashboardController(MatrixIncDbContext context)
        {
            _context = context;
        }

        /*
         * Deze actie toont het dashboard met alle statistieken en grafieken
         * Haalt data op uit de database en vult het viewmodel
         */
        public IActionResult Index()
        {
            // Orders ophalen inclusief gekoppelde producten
            var ordersWithProducts = _context.Orders
                .Include(o => o.OrderProducts)
                .ToList();

            // ViewModel vullen met totalen
            var viewModel = new DashboardViewModel
            {
                // Totaal aantal producten ophalen
                TotalProducts = _context.Products.Count(),
                // Totaal aantal actieve klanten ophalen
                TotalCustomers = _context.Customers.Where(c => c.Active).Count(),
                // Totaal aantal bestellingen ophalen
                TotalOrders = ordersWithProducts.Count,
                // Totale omzet berekenen door alle productprijzen bij elkaar op te tellen
                TotalRevenue = ordersWithProducts
                    .SelectMany(o => o.OrderProducts)
                    .Sum(p => p.ProductPrice)
            };

            // Prijsverdeling van producten maken voor grafiek
            var priceRanges = new[] { 0m, 100m, 500m, 1000m, 5000m, decimal.MaxValue };
            var products = _context.Products.ToList(); 
            var distribution = new List<ChartDataPoint>();
            
            for (int i = 0; i < priceRanges.Length - 1; i++)
            {
                var currentMin = priceRanges[i];
                var currentMax = priceRanges[i + 1];
                // Aantal producten binnen deze prijsklasse tellen
                var count = products.Count(p => 
                    p.Price >= currentMin && p.Price < currentMax);
                var label = $"€{currentMin} - {(currentMax == decimal.MaxValue ? "+" : "€" + currentMax.ToString())}";
                distribution.Add(new ChartDataPoint { Label = label, Value = count });
            }
            // Prijsverdeling toevoegen aan het viewmodel
            viewModel.ProductPriceDistribution = distribution;

            // Orders per dag van de afgelopen 7 dagen voor grafiek
            var last7Days = Enumerable.Range(0, 7)
                .Select(i => DateTime.Now.Date.AddDays(-i))
                .Reverse()
                .ToList();

            viewModel.OrdersOverTime = last7Days.Select(date => new ChartDataPoint
            {
                Label = date.ToString("MM/dd"),
                // Aantal orders op deze dag tellen
                Value = ordersWithProducts.Count(o => o.OrderDate.Date == date)
            }).ToList();

            // Top 5 producten op basis van aantal bestellingen
            var productsWithOrderProducts = _context.OrderProducts
                .Include(op => op.Product)
                .ToList(); // Alle order-product combinaties ophalen

            viewModel.TopProducts = productsWithOrderProducts
                .GroupBy(op => op.ProductName) 
                .Select(g => new TopProductViewModel
                {
                    ProductName = g.Key, 
                    OrderCount = g.Count(),
                    TotalRevenue = g.Sum(op => op.ProductPrice * op.Quantity)
                })
                .OrderByDescending(p => p.OrderCount)
                .Take(5)
                .ToList();

            // View tonen met het gevulde viewmodel
            return View(viewModel);
        }
    }
} 

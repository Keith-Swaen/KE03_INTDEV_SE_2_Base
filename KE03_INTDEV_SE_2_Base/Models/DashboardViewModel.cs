using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KE03_INTDEV_SE_2_Base.Models
{
    public class DashboardViewModel
    {
        public int TotalProducts { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }

        public List<ChartDataPoint> ProductPriceDistribution { get; set; }
        public List<ChartDataPoint> OrdersOverTime { get; set; }
        public List<TopProductViewModel> TopProducts { get; set; }

        public DashboardViewModel()
        {
            ProductPriceDistribution = new List<ChartDataPoint>();
            OrdersOverTime = new List<ChartDataPoint>();
            TopProducts = new List<TopProductViewModel>();
        }
    }

    public class ChartDataPoint
    {
        [Required]
        public string Label { get; set; } = string.Empty;
        public double Value { get; set; }
    }

    public class TopProductViewModel
    {
        [Required]
        public string ProductName { get; set; } = string.Empty;
        public int OrderCount { get; set; }
        public decimal TotalRevenue { get; set; }
    }
} 
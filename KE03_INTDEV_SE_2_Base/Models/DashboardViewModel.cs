using DataAccessLayer.Models;

namespace KE03_INTDEV_SE_2_Base.Models
{
    public class DashboardViewModel
    {
        public int TotalProducts { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        public List<Order> RecentOrders { get; set; } = new List<Order>();
        public List<CategoryCount> ProductsByCategory { get; set; } = new List<CategoryCount>();
        public List<TimelineData> OrdersTimeline { get; set; } = new List<TimelineData>();
        public List<ProductData> TopProducts { get; set; } = new List<ProductData>();
    }

    public class CategoryCount
    {
        public string Category { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class TimelineData
    {
        public DateTime Date { get; set; }
        public int Count { get; set; }
    }

    public class ProductData
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
} 
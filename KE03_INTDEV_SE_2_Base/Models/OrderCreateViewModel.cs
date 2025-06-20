using Microsoft.AspNetCore.Mvc.Rendering;

namespace KE03_INTDEV_SE_2_Base.Models
{
    public class OrderCreateViewModel
    {
        public int CustomerId { get; set; }
        public List<OrderProductViewModel> OrderProducts { get; set; } = new();

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public IEnumerable<SelectListItem>? Customers { get; set; }
        public IEnumerable<SelectListItem>? Products { get; set; }
    }

    public class OrderProductViewModel
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class OrderProduct
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public int? ProductId { get; set; }
        public Product Product { get; set; } = null!;

        // Gekopieerde velden van het product op moment van bestelling
        public string ProductName { get; set; } = string.Empty;
        public decimal ProductPrice { get; set; }

        public int Quantity { get; set; } = 1;
    }
}

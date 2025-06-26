using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    // Model voor een product, bevat info zoals naam, prijs, voorraad en categorie.
    public class Product
    {        
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }

        // Categorie ID voor de relatie met Category
        public int? CategoryId { get; set; }
        
        // Navigatie property voor de categorie
        public virtual Category? Category { get; set; }

        public int StockQuantity { get; set; }

        // Navigatie properties voor orders en onderdelen
        //public virtual ICollection<Order> Orders { get; } = new List<Order>();
        public virtual ICollection<OrderProduct> OrderProducts { get; } = new List<OrderProduct>();

        public virtual ICollection<Part> Parts { get; } = new List<Part>();
    }
}

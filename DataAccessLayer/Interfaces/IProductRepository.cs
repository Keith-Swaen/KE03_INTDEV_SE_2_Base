using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    // Interface voor producten: ophalen, toevoegen, aanpassen, verwijderen en categorieën koppelen.
    // IEnumerable betekent dat je een verzameling krijgt waar je overheen kunt lopen met foreach, zoals een lijst of een array.
    public interface IProductRepository
    {
        public IEnumerable<Product> GetAllProducts();

        public Product? GetProductById(int id);

        public void AddProduct(Product product);

        public void UpdateProduct(Product product);

        public void DeleteProduct(Product product);
        
        Task<bool> AddCategoryToProductAsync(int productId, int categoryId);
    }
}

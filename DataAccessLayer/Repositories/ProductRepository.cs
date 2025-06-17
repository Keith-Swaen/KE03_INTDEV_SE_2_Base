using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly MatrixIncDbContext _context;
        private readonly ILogger<ProductRepository> _logger;

        public ProductRepository(MatrixIncDbContext context, ILogger<ProductRepository> logger) 
        {
            _context = context;
            _logger = logger;
        }
        public void AddProduct(Product product)
        {
            try
            {
                _logger.LogInformation("Product toevoegen: {ProductName}", product.Name);
                _context.Products.Add(product);
                _context.SaveChanges();
                _logger.LogInformation("Product succesvol toegevoegd met ID: {ProductId}", product.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fout bij het toevoegen van product: {ProductName}", product.Name);
                throw;
            }
        }

        public void DeleteProduct(Product product)
        {
            try
            {
                _logger.LogInformation("Product verwijderen: {ProductId}", product.Id);
                _context.Products.Remove(product);
                _context.SaveChanges();
                _logger.LogInformation("Product succesvol verwijderd: {ProductId}", product.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fout bij het verwijderen van product: {ProductId}", product.Id);
                throw;
            }
        }

        public IEnumerable<Product> GetAllProducts()
        {
            _logger.LogInformation("Alle producten ophalen");
            return _context.Products
                .Include(p => p.Parts)
                .Include(p => p.Category);
        }

        public Product? GetProductById(int id)
        {
            _logger.LogInformation("Product ophalen met ID: {ProductId}", id);
            return _context.Products
                .Include(p => p.Parts)
                .Include(p => p.Category)
                .FirstOrDefault(p => p.Id == id);
        }

        public void UpdateProduct(Product product)
        {
            try
            {
                _logger.LogInformation("Product bijwerken: {ProductId}", product.Id);
                _context.Products.Update(product);
                _context.SaveChanges();
                _logger.LogInformation("Product succesvol bijgewerkt: {ProductId}", product.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fout bij het bijwerken van product: {ProductId}", product.Id);
                throw;
            }
        }

        // Nieuwe methode om een categorie toe te voegen aan een product
        public async Task<bool> AddCategoryToProductAsync(int productId, int categoryId)
        {
            try
            {
                _logger.LogInformation("Categorie {CategoryId} toevoegen aan product {ProductId}", categoryId, productId);
                
                var product = await _context.Products.FindAsync(productId);
                if (product == null)
                {
                    _logger.LogWarning("Product niet gevonden: {ProductId}", productId);
                    return false;
                }

                var category = await _context.Categories.FindAsync(categoryId);
                if (category == null)
                {
                    _logger.LogWarning("Categorie niet gevonden: {CategoryId}", categoryId);
                    return false;
                }

                product.CategoryId = categoryId;
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Categorie succesvol toegevoegd aan product: {ProductId}", productId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fout bij het toevoegen van categorie aan product: {ProductId}", productId);
                throw;
            }
        }
    }
}

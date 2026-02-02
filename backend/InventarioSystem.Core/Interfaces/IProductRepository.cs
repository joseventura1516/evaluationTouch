using InventarioSystem.Core.Entities;

namespace InventarioSystem.Core.Interfaces
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(int id);
        Task<IEnumerable<Product>> GetAllAsync();
        Task<IEnumerable<Product>> GetByCategoryAsync(string category);
        Task<IEnumerable<Product>> SearchAsync(string? searchTerm, string? category);
        Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold = 5);
        Task<Product> CreateAsync(Product product);
        Task<Product> UpdateAsync(Product product);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<string>> GetCategoriesAsync();
    }
}

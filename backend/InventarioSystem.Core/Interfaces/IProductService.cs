using InventarioSystem.Core.DTOs;

namespace InventarioSystem.Core.Interfaces
{
    public interface IProductService
    {
        Task<ServiceResponse<IEnumerable<ProductDto>>> GetAllProductsAsync();
        Task<ServiceResponse<ProductDto>> GetProductByIdAsync(int id);
        Task<ServiceResponse<IEnumerable<ProductDto>>> SearchProductsAsync(string? searchTerm, string? category);
        Task<ServiceResponse<IEnumerable<ProductDto>>> GetLowStockProductsAsync();
        Task<ServiceResponse<ProductDto>> CreateProductAsync(ProductCreateDto productDto, int userId);
        Task<ServiceResponse<ProductDto>> UpdateProductAsync(int id, ProductUpdateDto productDto);
        Task<ServiceResponse<bool>> DeleteProductAsync(int id);
        Task<ServiceResponse<IEnumerable<string>>> GetCategoriesAsync();
        Task<ServiceResponse<bool>> ReportLowStockAsync(int productId, int reportedByUserId);
    }
}

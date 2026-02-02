using InventarioSystem.Core.DTOs;
using InventarioSystem.Core.Entities;
using InventarioSystem.Core.Interfaces;

namespace InventarioSystem.Core.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly INotificationService _notificationService;
        private const int LowStockThreshold = 5;

        public ProductService(IProductRepository productRepository, INotificationService notificationService)
        {
            _productRepository = productRepository;
            _notificationService = notificationService;
        }

        public async Task<ServiceResponse<IEnumerable<ProductDto>>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllAsync();
            var productDtos = products.Select(MapToDto);
            return ServiceResponse<IEnumerable<ProductDto>>.SuccessResponse(productDtos);
        }

        public async Task<ServiceResponse<ProductDto>> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
            {
                return ServiceResponse<ProductDto>.FailureResponse("Producto no encontrado");
            }

            return ServiceResponse<ProductDto>.SuccessResponse(MapToDto(product));
        }

        public async Task<ServiceResponse<IEnumerable<ProductDto>>> SearchProductsAsync(string? searchTerm, string? category)
        {
            var products = await _productRepository.SearchAsync(searchTerm, category);
            var productDtos = products.Select(MapToDto);
            return ServiceResponse<IEnumerable<ProductDto>>.SuccessResponse(productDtos);
        }

        public async Task<ServiceResponse<IEnumerable<ProductDto>>> GetLowStockProductsAsync()
        {
            var products = await _productRepository.GetLowStockProductsAsync(LowStockThreshold);
            var productDtos = products.Select(MapToDto);
            return ServiceResponse<IEnumerable<ProductDto>>.SuccessResponse(productDtos);
        }

        public async Task<ServiceResponse<ProductDto>> CreateProductAsync(ProductCreateDto productDto, int userId)
        {
            var product = new Product
            {
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                Quantity = productDto.Quantity,
                Category = productDto.Category,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var createdProduct = await _productRepository.CreateAsync(product);

            // Verificar si el stock es bajo y notificar a los administradores
            if (createdProduct.Quantity < LowStockThreshold)
            {
                await _notificationService.NotifyAdministratorsAsync(
                    $"Alerta: El producto '{createdProduct.Name}' tiene inventario bajo ({createdProduct.Quantity} unidades)",
                    "Warning"
                );
            }

            return ServiceResponse<ProductDto>.SuccessResponse(MapToDto(createdProduct), "Producto creado exitosamente");
        }

        public async Task<ServiceResponse<ProductDto>> UpdateProductAsync(int id, ProductUpdateDto productDto)
        {
            var existingProduct = await _productRepository.GetByIdAsync(id);

            if (existingProduct == null)
            {
                return ServiceResponse<ProductDto>.FailureResponse("Producto no encontrado");
            }

            var previousQuantity = existingProduct.Quantity;

            existingProduct.Name = productDto.Name;
            existingProduct.Description = productDto.Description;
            existingProduct.Price = productDto.Price;
            existingProduct.Quantity = productDto.Quantity;
            existingProduct.Category = productDto.Category;
            existingProduct.UpdatedAt = DateTime.UtcNow;

            var updatedProduct = await _productRepository.UpdateAsync(existingProduct);

            // Notificar si el stock bajó a menos del umbral
            if (previousQuantity >= LowStockThreshold && updatedProduct.Quantity < LowStockThreshold)
            {
                await _notificationService.NotifyAdministratorsAsync(
                    $"Alerta: El producto '{updatedProduct.Name}' tiene inventario bajo ({updatedProduct.Quantity} unidades)",
                    "Warning"
                );
            }

            return ServiceResponse<ProductDto>.SuccessResponse(MapToDto(updatedProduct), "Producto actualizado exitosamente");
        }

        public async Task<ServiceResponse<bool>> DeleteProductAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
            {
                return ServiceResponse<bool>.FailureResponse("Producto no encontrado");
            }

            await _productRepository.DeleteAsync(id);
            return ServiceResponse<bool>.SuccessResponse(true, "Producto eliminado exitosamente");
        }

        public async Task<ServiceResponse<IEnumerable<string>>> GetCategoriesAsync()
        {
            var categories = await _productRepository.GetCategoriesAsync();
            return ServiceResponse<IEnumerable<string>>.SuccessResponse(categories);
        }

        public async Task<ServiceResponse<bool>> ReportLowStockAsync(int productId, int reportedByUserId)
        {
            var product = await _productRepository.GetByIdAsync(productId);

            if (product == null)
            {
                return ServiceResponse<bool>.FailureResponse("Producto no encontrado");
            }

            await _notificationService.NotifyAdministratorsAsync(
                $"Reporte de empleado: El producto '{product.Name}' requiere reabastecimiento (Stock actual: {product.Quantity} unidades)",
                "Warning"
            );

            return ServiceResponse<bool>.SuccessResponse(true, "Reporte enviado exitosamente a los administradores");
        }

        private static ProductDto MapToDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Quantity = product.Quantity,
                Category = product.Category,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt,
                IsLowStock = product.Quantity < LowStockThreshold
            };
        }
    }
}

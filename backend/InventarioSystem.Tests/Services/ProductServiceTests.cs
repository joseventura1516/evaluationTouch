using FluentAssertions;
using InventarioSystem.Core.DTOs;
using InventarioSystem.Core.Entities;
using InventarioSystem.Core.Interfaces;
using InventarioSystem.Core.Services;
using Moq;
using Xunit;

namespace InventarioSystem.Tests.Services
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly Mock<INotificationService> _notificationServiceMock;
        private readonly ProductService _productService;

        public ProductServiceTests()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _notificationServiceMock = new Mock<INotificationService>();
            _productService = new ProductService(_productRepositoryMock.Object, _notificationServiceMock.Object);
        }

        [Fact]
        public async Task GetAllProductsAsync_ReturnsAllProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Product 1", Price = 100, Quantity = 10, Category = "Cat1" },
                new Product { Id = 2, Name = "Product 2", Price = 200, Quantity = 5, Category = "Cat2" }
            };

            _productRepositoryMock.Setup(r => r.GetAllAsync())
                .ReturnsAsync(products);

            // Act
            var result = await _productService.GetAllProductsAsync();

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetProductByIdAsync_WithExistingId_ReturnsProduct()
        {
            // Arrange
            var product = new Product
            {
                Id = 1,
                Name = "Test Product",
                Description = "Test Description",
                Price = 99.99m,
                Quantity = 10,
                Category = "Test Category"
            };

            _productRepositoryMock.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(product);

            // Act
            var result = await _productService.GetProductByIdAsync(1);

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.Name.Should().Be("Test Product");
        }

        [Fact]
        public async Task GetProductByIdAsync_WithNonExistingId_ReturnsFailure()
        {
            // Arrange
            _productRepositoryMock.Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((Product?)null);

            // Act
            var result = await _productService.GetProductByIdAsync(999);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("no encontrado");
        }

        [Fact]
        public async Task CreateProductAsync_WithValidData_ReturnsCreatedProduct()
        {
            // Arrange
            var productDto = new ProductCreateDto
            {
                Name = "New Product",
                Description = "New Description",
                Price = 50.00m,
                Quantity = 20,
                Category = "New Category"
            };

            _productRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<Product>()))
                .ReturnsAsync((Product p) =>
                {
                    p.Id = 1;
                    return p;
                });

            // Act
            var result = await _productService.CreateProductAsync(productDto, 1);

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.Name.Should().Be("New Product");
            result.Message.Should().Contain("creado exitosamente");
        }

        [Fact]
        public async Task CreateProductAsync_WithLowStock_NotifiesAdministrators()
        {
            // Arrange
            var productDto = new ProductCreateDto
            {
                Name = "Low Stock Product",
                Description = "Description",
                Price = 50.00m,
                Quantity = 3, // Less than 5
                Category = "Category"
            };

            _productRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<Product>()))
                .ReturnsAsync((Product p) =>
                {
                    p.Id = 1;
                    return p;
                });

            // Act
            var result = await _productService.CreateProductAsync(productDto, 1);

            // Assert
            result.Success.Should().BeTrue();
            _notificationServiceMock.Verify(
                n => n.NotifyAdministratorsAsync(It.IsAny<string>(), It.IsAny<string>()),
                Times.Once);
        }

        [Fact]
        public async Task UpdateProductAsync_WithExistingProduct_ReturnsUpdatedProduct()
        {
            // Arrange
            var existingProduct = new Product
            {
                Id = 1,
                Name = "Old Name",
                Description = "Old Description",
                Price = 100m,
                Quantity = 10,
                Category = "Old Category"
            };

            var updateDto = new ProductUpdateDto
            {
                Name = "Updated Name",
                Description = "Updated Description",
                Price = 150m,
                Quantity = 15,
                Category = "Updated Category"
            };

            _productRepositoryMock.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(existingProduct);

            _productRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Product>()))
                .ReturnsAsync((Product p) => p);

            // Act
            var result = await _productService.UpdateProductAsync(1, updateDto);

            // Assert
            result.Success.Should().BeTrue();
            result.Data!.Name.Should().Be("Updated Name");
            result.Data.Price.Should().Be(150m);
        }

        [Fact]
        public async Task UpdateProductAsync_WithNonExistingProduct_ReturnsFailure()
        {
            // Arrange
            var updateDto = new ProductUpdateDto
            {
                Name = "Updated Name",
                Description = "Updated Description",
                Price = 150m,
                Quantity = 15,
                Category = "Updated Category"
            };

            _productRepositoryMock.Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((Product?)null);

            // Act
            var result = await _productService.UpdateProductAsync(999, updateDto);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("no encontrado");
        }

        [Fact]
        public async Task DeleteProductAsync_WithExistingProduct_ReturnsSuccess()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "To Delete" };

            _productRepositoryMock.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(product);

            _productRepositoryMock.Setup(r => r.DeleteAsync(1))
                .ReturnsAsync(true);

            // Act
            var result = await _productService.DeleteProductAsync(1);

            // Assert
            result.Success.Should().BeTrue();
            result.Message.Should().Contain("eliminado exitosamente");
        }

        [Fact]
        public async Task DeleteProductAsync_WithNonExistingProduct_ReturnsFailure()
        {
            // Arrange
            _productRepositoryMock.Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((Product?)null);

            // Act
            var result = await _productService.DeleteProductAsync(999);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("no encontrado");
        }

        [Fact]
        public async Task GetLowStockProductsAsync_ReturnsOnlyLowStockProducts()
        {
            // Arrange
            var lowStockProducts = new List<Product>
            {
                new Product { Id = 1, Name = "Low Stock 1", Quantity = 2 },
                new Product { Id = 2, Name = "Low Stock 2", Quantity = 4 }
            };

            _productRepositoryMock.Setup(r => r.GetLowStockProductsAsync(5))
                .ReturnsAsync(lowStockProducts);

            // Act
            var result = await _productService.GetLowStockProductsAsync();

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().HaveCount(2);
            result.Data!.All(p => p.IsLowStock).Should().BeTrue();
        }

        [Fact]
        public async Task SearchProductsAsync_WithSearchTerm_ReturnsFilteredProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Laptop HP", Category = "Electronics" }
            };

            _productRepositoryMock.Setup(r => r.SearchAsync("Laptop", null))
                .ReturnsAsync(products);

            // Act
            var result = await _productService.SearchProductsAsync("Laptop", null);

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().HaveCount(1);
            result.Data!.First().Name.Should().Contain("Laptop");
        }

        [Fact]
        public async Task GetCategoriesAsync_ReturnsDistinctCategories()
        {
            // Arrange
            var categories = new List<string> { "Electronics", "Accessories", "Audio" };

            _productRepositoryMock.Setup(r => r.GetCategoriesAsync())
                .ReturnsAsync(categories);

            // Act
            var result = await _productService.GetCategoriesAsync();

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().HaveCount(3);
        }
    }
}

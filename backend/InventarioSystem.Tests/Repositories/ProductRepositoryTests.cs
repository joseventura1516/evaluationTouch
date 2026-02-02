using FluentAssertions;
using InventarioSystem.Core.Entities;
using InventarioSystem.Infrastructure.Data;
using InventarioSystem.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace InventarioSystem.Tests.Repositories
{
    public class ProductRepositoryTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly ProductRepository _repository;

        public ProductRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _repository = new ProductRepository(_context);

            SeedDatabase();
        }

        private void SeedDatabase()
        {
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Product 1", Description = "Desc 1", Price = 100, Quantity = 10, Category = "Electronics", IsActive = true },
                new Product { Id = 2, Name = "Product 2", Description = "Desc 2", Price = 200, Quantity = 3, Category = "Electronics", IsActive = true },
                new Product { Id = 3, Name = "Product 3", Description = "Desc 3", Price = 300, Quantity = 20, Category = "Accessories", IsActive = true },
                new Product { Id = 4, Name = "Deleted Product", Description = "Desc 4", Price = 400, Quantity = 5, Category = "Electronics", IsActive = false }
            };

            _context.Products.AddRange(products);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetAllAsync_ReturnsOnlyActiveProducts()
        {
            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            result.Should().HaveCount(3);
            result.Should().OnlyContain(p => p.IsActive);
        }

        [Fact]
        public async Task GetByIdAsync_WithExistingId_ReturnsProduct()
        {
            // Act
            var result = await _repository.GetByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("Product 1");
        }

        [Fact]
        public async Task GetByIdAsync_WithInactiveProduct_ReturnsNull()
        {
            // Act
            var result = await _repository.GetByIdAsync(4);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetByCategoryAsync_ReturnsProductsInCategory()
        {
            // Act
            var result = await _repository.GetByCategoryAsync("Electronics");

            // Assert
            result.Should().HaveCount(2);
            result.Should().OnlyContain(p => p.Category == "Electronics");
        }

        [Fact]
        public async Task SearchAsync_WithSearchTerm_ReturnsMatchingProducts()
        {
            // Act
            var result = await _repository.SearchAsync("Product 1", null);

            // Assert
            result.Should().HaveCount(1);
            result.First().Name.Should().Be("Product 1");
        }

        [Fact]
        public async Task SearchAsync_WithCategory_ReturnsMatchingProducts()
        {
            // Act
            var result = await _repository.SearchAsync(null, "Accessories");

            // Assert
            result.Should().HaveCount(1);
            result.First().Category.Should().Be("Accessories");
        }

        [Fact]
        public async Task GetLowStockProductsAsync_ReturnsProductsBelowThreshold()
        {
            // Act
            var result = await _repository.GetLowStockProductsAsync(5);

            // Assert
            result.Should().HaveCount(1);
            result.First().Quantity.Should().BeLessThan(5);
        }

        [Fact]
        public async Task CreateAsync_AddsNewProduct()
        {
            // Arrange
            var newProduct = new Product
            {
                Name = "New Product",
                Description = "New Desc",
                Price = 500,
                Quantity = 15,
                Category = "New Category",
                IsActive = true
            };

            // Act
            var result = await _repository.CreateAsync(newProduct);

            // Assert
            result.Id.Should().BeGreaterThan(0);
            var saved = await _context.Products.FindAsync(result.Id);
            saved.Should().NotBeNull();
            saved!.Name.Should().Be("New Product");
        }

        [Fact]
        public async Task UpdateAsync_ModifiesExistingProduct()
        {
            // Arrange
            var product = await _context.Products.FindAsync(1);
            product!.Name = "Updated Name";
            product.Price = 999;

            // Act
            var result = await _repository.UpdateAsync(product);

            // Assert
            result.Name.Should().Be("Updated Name");
            result.Price.Should().Be(999);
            result.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public async Task DeleteAsync_SoftDeletesProduct()
        {
            // Act
            var result = await _repository.DeleteAsync(1);

            // Assert
            result.Should().BeTrue();
            var product = await _context.Products.FindAsync(1);
            product!.IsActive.Should().BeFalse();
        }

        [Fact]
        public async Task ExistsAsync_WithExistingProduct_ReturnsTrue()
        {
            // Act
            var result = await _repository.ExistsAsync(1);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsAsync_WithInactiveProduct_ReturnsFalse()
        {
            // Act
            var result = await _repository.ExistsAsync(4);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task GetCategoriesAsync_ReturnsDistinctCategories()
        {
            // Act
            var result = await _repository.GetCategoriesAsync();

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain("Electronics");
            result.Should().Contain("Accessories");
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}

using Domain.Entities.ProductModule;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Persistence.Data;
using Persistence.Repositories;
using Xunit;

namespace Tests.Repositories
{
    /// <summary>
    /// Integration-style tests for ProductRepository.
    /// Tests product-specific repository operations using mocked DbContext.
    /// Verifies inherited GenericRepository functionality in Product context.
    /// </summary>
    public class ProductRepositoryTests
    {
        [Fact]
        public async Task AddProduct_WithValidData_CallsDbSetAddAsync()
        {
            // Arrange
            var mockDbSet = new Mock<DbSet<Product>>();
            var mockDbContext = new Mock<StoreDbContext>(new DbContextOptions<StoreDbContext>());
            mockDbContext.Setup(c => c.Set<Product>()).Returns(mockDbSet.Object);

            var repository = new ProductRepository(mockDbContext.Object);
            var product = new Product
            {
                Name = "Fresh Apples",
                Description = "Premium organic apples",
                Price = 15.99m,
                PictureUrl = "/images/apples.jpg"
            };

            // Act
            await repository.AddAsync(product);

            // Assert
            mockDbSet.Verify(d => d.AddAsync(product, default), Times.Once);
        }

        [Fact]
        public async Task DeleteProduct_WithExistingProduct_CallsDbSetRemove()
        {
            // Arrange
            var mockDbSet = new Mock<DbSet<Product>>();
            var mockDbContext = new Mock<StoreDbContext>(new DbContextOptions<StoreDbContext>());
            mockDbContext.Setup(c => c.Set<Product>()).Returns(mockDbSet.Object);

            var repository = new ProductRepository(mockDbContext.Object);
            var product = new Product
            {
                Id = 1,
                Name = "Apples",
                Description = "Fresh",
                Price = 10.00m,
                PictureUrl = "/images/apples.jpg"
            };

            // Act
            repository.Delete(product);

            // Assert
            mockDbSet.Verify(d => d.Remove(product), Times.Once);
        }

        [Fact]
        public void UpdateProduct_WithValidData_CallsDbSetUpdate()
        {
            // Arrange
            var mockDbSet = new Mock<DbSet<Product>>();
            var mockDbContext = new Mock<StoreDbContext>(new DbContextOptions<StoreDbContext>());
            mockDbContext.Setup(c => c.Set<Product>()).Returns(mockDbSet.Object);

            var repository = new ProductRepository(mockDbContext.Object);
            var product = new Product
            {
                Id = 1,
                Name = "Bananas",
                Description = "Yellow bananas",
                Price = 5.99m,
                PictureUrl = "/images/bananas.jpg"
            };

            // Act
            repository.Update(product);

            // Assert
            mockDbSet.Verify(d => d.Update(product), Times.Once);
        }

        [Fact]
        public async Task GetProductById_WithExistingProduct_ReturnsProduct()
        {
            // Arrange
            var expectedProduct = new Product
            {
                Id = 5,
                Name = "Oranges",
                Description = "Sweet oranges",
                Price = 8.99m,
                PictureUrl = "/images/oranges.jpg"
            };

            var mockDbSet = new Mock<DbSet<Product>>();
            mockDbSet.Setup(d => d.FindAsync(It.IsAny<object[]>()))
                .ReturnsAsync(expectedProduct);

            var mockDbContext = new Mock<StoreDbContext>(new DbContextOptions<StoreDbContext>());
            mockDbContext.Setup(c => c.Set<Product>()).Returns(mockDbSet.Object);

            var repository = new ProductRepository(mockDbContext.Object);

            // Act
            var result = await repository.GetByIdAsync(5);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(5);
            result.Name.Should().Be("Oranges");
        }

        [Fact]
        public async Task GetProductById_WithNonExistentId_ReturnsNull()
        {
            // Arrange
            var mockDbSet = new Mock<DbSet<Product>>();
            mockDbSet.Setup(d => d.FindAsync(It.IsAny<object[]>()))
                .ReturnsAsync((Product)null);

            var mockDbContext = new Mock<StoreDbContext>(new DbContextOptions<StoreDbContext>());
            mockDbContext.Setup(c => c.Set<Product>()).Returns(mockDbSet.Object);

            var repository = new ProductRepository(mockDbContext.Object);

            // Act
            var result = await repository.GetByIdAsync(999);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAllProducts_WithMultipleProducts_ReturnsAll()
        {
            // Arrange
            var products = new[]
            {
                new Product { Id = 1, Name = "Apples", Description = "Fresh apples", Price = 5.99m, PictureUrl = "/images/apples.jpg" },
                new Product { Id = 2, Name = "Bananas", Description = "Yellow bananas", Price = 3.99m, PictureUrl = "/images/bananas.jpg" },
                new Product { Id = 3, Name = "Oranges", Description = "Sweet oranges", Price = 4.99m, PictureUrl = "/images/oranges.jpg" }
            };

            var mockQueryable = products.AsQueryable().BuildMockDbSet();
            var mockDbContext = new Mock<StoreDbContext>(new DbContextOptions<StoreDbContext>());
            mockDbContext.Setup(c => c.Set<Product>()).Returns(mockQueryable.Object);

            var repository = new ProductRepository(mockDbContext.Object);

            // Act
            var result = await repository.GetAllAsync();

            // Assert
            result.Should().HaveCount(3);
            result.Should().Contain(p => p.Name == "Apples");
            result.Should().Contain(p => p.Name == "Bananas");
            result.Should().Contain(p => p.Name == "Oranges");
        }

        [Fact]
        public async Task GetAllProducts_WithEmptyDatabase_ReturnsEmptyList()
        {
            // Arrange
            var products = new Product[0].AsQueryable();
            var mockQueryable = products.BuildMockDbSet();
            var mockDbContext = new Mock<StoreDbContext>(new DbContextOptions<StoreDbContext>());
            mockDbContext.Setup(c => c.Set<Product>()).Returns(mockQueryable.Object);

            var repository = new ProductRepository(mockDbContext.Object);

            // Act
            var result = await repository.GetAllAsync();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task ProductOperations_AddUpdateDelete_MaintainsConsistency()
        {
            // Arrange - Product data
            var product = new Product
            {
                Id = 10,
                Name = "Strawberries",
                Description = "Fresh strawberries",
                Price = 9.99m,
                PictureUrl = "/images/strawberries.jpg"
            };

            var mockDbSet = new Mock<DbSet<Product>>();
            var mockDbContext = new Mock<StoreDbContext>(new DbContextOptions<StoreDbContext>());
            mockDbContext.Setup(c => c.Set<Product>()).Returns(mockDbSet.Object);

            var repository = new ProductRepository(mockDbContext.Object);

            // Act & Assert - Add
            await repository.AddAsync(product);
            mockDbSet.Verify(d => d.AddAsync(product, default), Times.Once);

            // Act & Assert - Update
            product.Price = 11.99m;
            repository.Update(product);
            mockDbSet.Verify(d => d.Update(product), Times.Once);

            // Act & Assert - Delete
            repository.Delete(product);
            mockDbSet.Verify(d => d.Remove(product), Times.Once);
        }

        [Fact]
        public async Task GetProductByVariousIds_WithDifferentProducts_ReturnsCorrectProduct()
        {
            // Arrange
            var products = new[]
            {
                new Product { Id = 1, Name = "Product 1", Description = "D1", Price = 10m, PictureUrl = "p1.jpg" },
                new Product { Id = 2, Name = "Product 2", Description = "D2", Price = 20m, PictureUrl = "p2.jpg" },
                new Product { Id = 3, Name = "Product 3", Description = "D3", Price = 30m, PictureUrl = "p3.jpg" }
            };

            var mockDbSet = new Mock<DbSet<Product>>();
            
            mockDbSet.Setup(d => d.FindAsync(It.Is<object[]>(x => x[0].Equals(2))))
                .ReturnsAsync(products[1]);

            var mockDbContext = new Mock<StoreDbContext>(new DbContextOptions<StoreDbContext>());
            mockDbContext.Setup(c => c.Set<Product>()).Returns(mockDbSet.Object);

            var repository = new ProductRepository(mockDbContext.Object);

            // Act
            var result = await repository.GetByIdAsync(2);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("Product 2");
            result.Price.Should().Be(20m);
        }
    }
}

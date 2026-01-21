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
    /// Integration-style tests for GenericRepository<TEntity, Tkey>.
    /// Tests CRUD operations using mocked DbContext.
    /// Focuses on repository contract compliance and business logic.
    /// </summary>
    public class GenericRepositoryTests
    {
        [Fact]
        public async Task AddAsync_WithValidProduct_CallsDbSetAddAsync()
        {
            // Arrange
            var mockDbSet = new Mock<DbSet<Product>>();
            var mockDbContext = new Mock<StoreDbContext>(new DbContextOptions<StoreDbContext>());
            mockDbContext.Setup(c => c.Set<Product>()).Returns(mockDbSet.Object);

            var repository = new GenericRepository<Product, int>(mockDbContext.Object);
            var product = new Product
            {
                Name = "Test Product",
                Description = "Test Description",
                Price = 99.99m,
                PictureUrl = "test.jpg"
            };

            // Act
            await repository.AddAsync(product);

            // Assert
            mockDbSet.Verify(d => d.AddAsync(product, default), Times.Once);
        }

        [Fact]
        public void Delete_WithValidProduct_CallsDbSetRemove()
        {
            // Arrange
            var mockDbSet = new Mock<DbSet<Product>>();
            var mockDbContext = new Mock<StoreDbContext>(new DbContextOptions<StoreDbContext>());
            mockDbContext.Setup(c => c.Set<Product>()).Returns(mockDbSet.Object);

            var repository = new GenericRepository<Product, int>(mockDbContext.Object);
            var product = new Product { Id = 1, Name = "Product", Description = "D", Price = 10m, PictureUrl = "p.jpg" };

            // Act
            repository.Delete(product);

            // Assert
            mockDbSet.Verify(d => d.Remove(product), Times.Once);
        }

        [Fact]
        public void Update_WithValidProduct_CallsDbSetUpdate()
        {
            // Arrange
            var mockDbSet = new Mock<DbSet<Product>>();
            var mockDbContext = new Mock<StoreDbContext>(new DbContextOptions<StoreDbContext>());
            mockDbContext.Setup(c => c.Set<Product>()).Returns(mockDbSet.Object);

            var repository = new GenericRepository<Product, int>(mockDbContext.Object);
            var product = new Product { Id = 1, Name = "Product", Description = "D", Price = 10m, PictureUrl = "p.jpg" };

            // Act
            repository.Update(product);

            // Assert
            mockDbSet.Verify(d => d.Update(product), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_CallsFindAsync()
        {
            // Arrange
            var expectedProduct = new Product { Id = 1, Name = "Test", Description = "D", Price = 10m, PictureUrl = "p.jpg" };
            var mockDbSet = new Mock<DbSet<Product>>();
            mockDbSet.Setup(d => d.FindAsync(It.IsAny<object[]>()))
                .ReturnsAsync(expectedProduct);

            var mockDbContext = new Mock<StoreDbContext>(new DbContextOptions<StoreDbContext>());
            mockDbContext.Setup(c => c.Set<Product>()).Returns(mockDbSet.Object);

            var repository = new GenericRepository<Product, int>(mockDbContext.Object);

            // Act
            var result = await repository.GetByIdAsync(1);

            // Assert
            result.Should().Be(expectedProduct);
            mockDbSet.Verify(d => d.FindAsync(It.IsAny<object[]>()), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_WithoutTracking_ReturnsAsNoTracking()
        {
            // Arrange
            var products = new[] { 
                new Product { Id = 1, Name = "P1", Description = "D1", Price = 10m, PictureUrl = "p1.jpg" },
                new Product { Id = 2, Name = "P2", Description = "D2", Price = 20m, PictureUrl = "p2.jpg" }
            };

            var mockQueryable = products.AsQueryable().BuildMockDbSet();
            var mockDbContext = new Mock<StoreDbContext>(new DbContextOptions<StoreDbContext>());
            mockDbContext.Setup(c => c.Set<Product>()).Returns(mockQueryable.Object);

            var repository = new GenericRepository<Product, int>(mockDbContext.Object);

            // Act
            var result = await repository.GetAllAsync(withTracking: false);

            // Assert
            result.Should().NotBeEmpty();
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAllAsync_WithTracking_ReturnsAllProducts()
        {
            // Arrange
            var products = new[] { 
                new Product { Id = 1, Name = "P1", Description = "D1", Price = 10m, PictureUrl = "p1.jpg" },
                new Product { Id = 2, Name = "P2", Description = "D2", Price = 20m, PictureUrl = "p2.jpg" }
            };

            var mockQueryable = products.AsQueryable().BuildMockDbSet();
            var mockDbContext = new Mock<StoreDbContext>(new DbContextOptions<StoreDbContext>());
            mockDbContext.Setup(c => c.Set<Product>()).Returns(mockQueryable.Object);

            var repository = new GenericRepository<Product, int>(mockDbContext.Object);

            // Act
            var result = await repository.GetAllAsync(withTracking: true);

            // Assert
            result.Should().NotBeEmpty();
            result.Should().HaveCount(2);
        }
    }
}

// Extension method for creating mock DbSet from IQueryable
public static class MockExtensions
{
    public static Mock<DbSet<T>> BuildMockDbSet<T>(this IQueryable<T> source) where T : class
    {
        var mockDbSet = new Mock<DbSet<T>>();
        mockDbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(source.Provider);
        mockDbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(source.Expression);
        mockDbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(source.ElementType);
        mockDbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(source.GetEnumerator());
        mockDbSet.As<IAsyncEnumerable<T>>()
            .Setup(m => m.GetAsyncEnumerator(It.IsAny<System.Threading.CancellationToken>()))
            .Returns(new AsyncEnumeratorWrapper<T>(source.GetEnumerator()));
        return mockDbSet;
    }
}

// Wrapper for async enumeration
public class AsyncEnumeratorWrapper<T> : IAsyncEnumerator<T>
{
    private readonly IEnumerator<T> _inner;

    public AsyncEnumeratorWrapper(IEnumerator<T> inner)
    {
        _inner = inner;
    }

    public T Current => _inner.Current;

    public async ValueTask DisposeAsync()
    {
        _inner?.Dispose();
        await ValueTask.CompletedTask;
    }

    public async ValueTask<bool> MoveNextAsync()
    {
        await Task.CompletedTask;
        return _inner.MoveNext();
    }
}

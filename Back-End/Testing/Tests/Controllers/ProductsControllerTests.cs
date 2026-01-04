using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Presentation.Controllers;
using ServiceAbstraction.Contracts;
using Shared;
using Shared.Dtos.ProductDto;
using Tests.Fixtures;
using Xunit;

namespace Tests.Controllers
{
    public class ProductsControllerTests : TestFixture
    {
        private readonly ProductsController _sut; // System Under Test
        private readonly Mock<IServiceManager> _mockServiceManager;

        public ProductsControllerTests()
        {
            _mockServiceManager = MockOf<IServiceManager>();
            _sut = new ProductsController(_mockServiceManager.Object);
        }

        #region GetAllProductsAsync Tests

        [Fact]
        public async Task GetAllProductsAsync_WithValidParams_ReturnsOkWithPaginatedResult()
        {
            // Arrange
            var queryParams = Fixture.Create<ProductQueryParams>();
            var expectedProducts = Fixture.CreateMany<ProductsResultDto>(5).ToList();
            PaginatedResult<ProductsResultDto> paginatedResult = new(
                pageSize: 5, pageIndex: 1, totalCount: 10, _data: expectedProducts
                );

            _mockServiceManager
                .Setup(sm => sm.ProductService.GetAllProductsAsync(queryParams))
                .ReturnsAsync(paginatedResult);

            // Act
            var result = await _sut.GetAllProductsAsync(queryParams);

            // Assert
            result.Result
                .Should()
                .BeOfType<OkObjectResult>();

            var okResult = result.Result as OkObjectResult;
            okResult.Value
                .Should()
                .Be(paginatedResult);

            _mockServiceManager.Verify(
                sm => sm.ProductService.GetAllProductsAsync(queryParams),
                Times.Once);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(1)]
        public async Task GetAllProductsAsync_WithDifferentPageNumbers_ReturnsPaginatedResult(int? pageNumber)
        {
            // Arrange
            var queryParams = new ProductQueryParams { PageIndex = pageNumber ?? 1 };
            var paginatedResult = Fixture.Create<PaginatedResult<ProductsResultDto>>();

            _mockServiceManager
                .Setup(sm => sm.ProductService.GetAllProductsAsync(It.IsAny<ProductQueryParams>()))
                .ReturnsAsync(paginatedResult);

            // Act
            var result = await _sut.GetAllProductsAsync(queryParams);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
        }

        #endregion

        #region GetAllBrandsAsync Tests

        [Fact]
        public async Task GetAllBrandsAsync_WithValidRequest_ReturnsOkWithBrandsList()
        {
            // Arrange
            var expectedBrands = Fixture.CreateMany<BrandResultDto>(3).ToList();

            _mockServiceManager
                .Setup(sm => sm.ProductService.GetAllBrandsAsync())
                .ReturnsAsync(expectedBrands);

            // Act
            var result = await _sut.GetAllBrandsAsync();

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();

            var okResult = result.Result as OkObjectResult;
            okResult.Value
                .Should()
                .BeEquivalentTo(expectedBrands);

            _mockServiceManager.Verify(
                sm => sm.ProductService.GetAllBrandsAsync(),
                Times.Once);
        }

        [Fact]
        public async Task GetAllBrandsAsync_WhenNoDataExists_ReturnsEmptyList()
        {
            // Arrange
            var emptyBrands = new List<BrandResultDto>();

            _mockServiceManager
                .Setup(sm => sm.ProductService.GetAllBrandsAsync())
                .ReturnsAsync(emptyBrands);

            // Act
            var result = await _sut.GetAllBrandsAsync();

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            ((IEnumerable<BrandResultDto>)okResult.Value).Should().BeEmpty();
        }

        #endregion

        #region GetAllResultAsync Tests

        [Fact]
        public async Task GetAllResultAsync_WithValidRequest_ReturnsOkWithTypesList()
        {
            // Arrange
            var expectedTypes = Fixture.CreateMany<TypeResultDto>(4).ToList();

            _mockServiceManager
                .Setup(sm => sm.ProductService.GetAllTypesAsync())
                .ReturnsAsync(expectedTypes);

            // Act
            var result = await _sut.GetAllResultAsync();

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();

            var okResult = result.Result as OkObjectResult;
            okResult.Value
                .Should()
                .BeEquivalentTo(expectedTypes);

            _mockServiceManager.Verify(
                sm => sm.ProductService.GetAllTypesAsync(),
                Times.Once);
        }

        #endregion

        #region GetProductByIdAsync Tests

        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(999)]
        public async Task GetProductByIdAsync_WithValidId_ReturnsOkWithProduct(int productId)
        {
            // Arrange
            var expectedProduct = Fixture.Create<ProductsResultDto>();

            _mockServiceManager
                .Setup(sm => sm.ProductService.GetProductByIdAsync(productId))
                .ReturnsAsync(expectedProduct);

            // Act
            var result = await _sut.GetProductByIdAsync(productId);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();

            var okResult = result.Result as OkObjectResult;
            okResult.Value
                .Should()
                .BeEquivalentTo(expectedProduct);

            _mockServiceManager.Verify(
                sm => sm.ProductService.GetProductByIdAsync(productId),
                Times.Once);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public async Task GetProductByIdAsync_WithInvalidId_CallsServiceWithInvalidId(int invalidId)
        {
            // Arrange
            var product = Fixture.Create<ProductsResultDto>();

            _mockServiceManager
                .Setup(sm => sm.ProductService.GetProductByIdAsync(invalidId))
                .ReturnsAsync(product);

            // Act
            var result = await _sut.GetProductByIdAsync(invalidId);

            // Assert
            _mockServiceManager.Verify(
                sm => sm.ProductService.GetProductByIdAsync(invalidId),
                Times.Once);
        }

        #endregion
    }
}
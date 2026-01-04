using AutoFixture;
using AutoMapper;
using Domain.Contracts;
using Domain.Entities.ProductModule;
using Domain.Exceptions;
using FluentAssertions;
using FluentValidation;
using Moq;
using Service.Implementations;
using Service.Specifications;
using ServiceAbstraction.Contracts;
using Shared;
using Shared.Dtos.AiSearch;
using Shared.Dtos.ProductDto;
using Tests.Fixtures;
using Xunit;

namespace Tests.Services
{
    public class ProductServiceTests : TestFixture
    {
        private readonly ProductService _sut;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<IValidator<UpdateProductDto>> _mockValidator;
        private readonly Mock<IFileService> _mockFileService;

        public ProductServiceTests()
        {
            _mockUnitOfWork = MockOf<IUnitOfWork>();
            _mockMapper = MockOf<IMapper>();
            _mockProductRepository = MockOf<IProductRepository>();
            _mockValidator = MockOf<IValidator<UpdateProductDto>>();
            _mockFileService = MockOf<IFileService>();

            _sut = new ProductService(
                _mockUnitOfWork.Object,
                _mockMapper.Object,
                _mockProductRepository.Object,
                _mockValidator.Object,
                _mockFileService.Object
            );
        }

        #region GetAllBrandsAsync Tests

        [Fact]
        public async Task GetAllBrandsAsync_WithValidData_ReturnsBrandDtos()
        {
            // Arrange
            var brands = Fixture.CreateMany<ProductBrand>(3).ToList();
            var brandDtos = Fixture.CreateMany<BrandResultDto>(3).ToList();

            var mockRepo = MockOf<IGenericRepository<ProductBrand, int>>();
            mockRepo.Setup(r => r.GetAllAsync(false)).ReturnsAsync(brands);

            _mockUnitOfWork
                .Setup(uw => uw.GetRepository<ProductBrand, int>())
                .Returns(mockRepo.Object);

            _mockMapper
                .Setup(m => m.Map<IEnumerable<ProductBrand>, IEnumerable<BrandResultDto>>(brands))
                .Returns(brandDtos);

            // Act
            var result = await _sut.GetAllBrandsAsync();

            // Assert
            result.Should().BeEquivalentTo(brandDtos);
            result.Should().HaveCount(3);

            mockRepo.Verify(r => r.GetAllAsync(false), Times.Once);
            _mockMapper.Verify(
                m => m.Map<IEnumerable<ProductBrand>, IEnumerable<BrandResultDto>>(It.IsAny<IEnumerable<ProductBrand>>()),
                Times.Once);
        }

        [Fact]
        public async Task GetAllBrandsAsync_WhenNoBrands_ReturnsEmptyList()
        {
            // Arrange
            var emptyBrands = new List<ProductBrand>();
            var emptyBrandDtos = new List<BrandResultDto>();

            var mockRepo = MockOf<IGenericRepository<ProductBrand, int>>();
            mockRepo.Setup(r => r.GetAllAsync(false)).ReturnsAsync(emptyBrands);

            _mockUnitOfWork
                .Setup(uw => uw.GetRepository<ProductBrand, int>())
                .Returns(mockRepo.Object);

            _mockMapper
                .Setup(m => m.Map<IEnumerable<ProductBrand>, IEnumerable<BrandResultDto>>(emptyBrands))
                .Returns(emptyBrandDtos);

            // Act
            var result = await _sut.GetAllBrandsAsync();

            // Assert
            result.Should().BeEmpty();
        }

        #endregion

        #region GetAllTypesAsync Tests

        [Fact]
        public async Task GetAllTypesAsync_WithValidData_ReturnsTypeDtos()
        {
            // Arrange
            var types = Fixture.CreateMany<ProductType>(4).ToList();
            var typeDtos = Fixture.CreateMany<TypeResultDto>(4).ToList();

            var mockRepo = MockOf<IGenericRepository<ProductType, int>>();
            mockRepo.Setup(r => r.GetAllAsync(false)).ReturnsAsync(types);

            _mockUnitOfWork
                .Setup(uw => uw.GetRepository<ProductType, int>())
                .Returns(mockRepo.Object);

            _mockMapper
                .Setup(m => m.Map<IEnumerable<TypeResultDto>>(types))
                .Returns(typeDtos);

            // Act
            var result = await _sut.GetAllTypesAsync();

            // Assert
            result.Should().BeEquivalentTo(typeDtos);
            result.Should().HaveCount(4);
        }

        #endregion

        #region GetAllProductsAsync Tests

        [Fact]
        public async Task GetAllProductsAsync_WithValidParams_ReturnsPaginatedResult()
        {
            // Arrange
            var queryParams = new ProductQueryParams { PageIndex = 1, PageSize = 10 };
            var products = Fixture.CreateMany<Product>(5).ToList();
            var productDtos = Fixture.CreateMany<ProductsResultDto>(5).ToList();

            var mockRepo = MockOf<IGenericRepository<Product, int>>();
            mockRepo.Setup(r => r.GetAllAsync(It.IsAny<ProductTypeAndBrandSpecifications>()))
                .ReturnsAsync(products);
            mockRepo.Setup(r => r.CountAsync(It.IsAny<ProductCountSpecification>()))
                .ReturnsAsync(20);

            _mockUnitOfWork
                .Setup(uw => uw.GetRepository<Product, int>())
                .Returns(mockRepo.Object);

            _mockMapper
                .Setup(m => m.Map<IEnumerable<Product>, IEnumerable<ProductsResultDto>>(products))
                .Returns(productDtos);

            // Act
            var result = await _sut.GetAllProductsAsync(queryParams);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().HaveCount(5);
            result.TotalCount.Should().Be(20);
            result.PageIndex.Should().Be(1);

            mockRepo.Verify(
                r => r.GetAllAsync(It.IsAny<ProductTypeAndBrandSpecifications>()),
                Times.Once);
            mockRepo.Verify(
                r => r.CountAsync(It.IsAny<ProductCountSpecification>()),
                Times.Once);
        }

        #endregion

        #region GetProductByIdAsync Tests

        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        public async Task GetProductByIdAsync_WithValidId_ReturnsProductDto(int productId)
        {
            // Arrange
            var product = Fixture.Create<Product>();
            var productDto = Fixture.Create<ProductsResultDto>();

            var mockRepo = MockOf<IGenericRepository<Product, int>>();
            mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<ProductTypeAndBrandSpecifications>()))
                .ReturnsAsync(product);

            _mockUnitOfWork
                .Setup(uw => uw.GetRepository<Product, int>())
                .Returns(mockRepo.Object);

            _mockMapper
                .Setup(m => m.Map<ProductsResultDto>(product))
                .Returns(productDto);

            // Act
            var result = await _sut.GetProductByIdAsync(productId);

            // Assert
            result.Should().BeEquivalentTo(productDto);
            mockRepo.Verify(
                r => r.GetByIdAsync(It.IsAny<ProductTypeAndBrandSpecifications>()),
                Times.Once);
        }

        [Fact]
        public async Task GetProductByIdAsync_WithInvalidId_ThrowsGenericNotFoundException()
        {
            // Arrange
            var invalidId = 999;
            var mockRepo = MockOf<IGenericRepository<Product, int>>();
            mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<ProductTypeAndBrandSpecifications>()))
                .ReturnsAsync((Product)null);

            _mockUnitOfWork
                .Setup(uw => uw.GetRepository<Product, int>())
                .Returns(mockRepo.Object);

            // Act
            var act = async () => await _sut.GetProductByIdAsync(invalidId);

            // Assert
            await act.Should().ThrowAsync<GenericNotFoundException<Product, int>>();
        }

        #endregion

        #region AddProduct Tests

        [Fact]
        public async Task AddProduct_WithValidDto_ReturnsProductId()
        {
            // Arrange
            var dto = Fixture.Create<CreatedProductDto>();
            var product = Fixture.Create<Product>();
            product.Id = 5;

            _mockMapper
                .Setup(m => m.Map<Product>(dto))
                .Returns(product);

            _mockProductRepository
                .Setup(r => r.AddAsync(It.IsAny<Product>()))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork
                .Setup(uw => uw.SaveChangeAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _sut.AddProduct(dto);

            // Assert
            result.Should().Be(5);
            _mockProductRepository.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Once);
            _mockUnitOfWork.Verify(uw => uw.SaveChangeAsync(), Times.Once);
        }

        #endregion

        #region DeleteProduct Tests

        [Fact]
        public async Task DeleteProduct_WithValidId_DeletesProductAndReturnsTrue()
        {
            // Arrange
            var productId = 5;
            var product = new Product
            {
                Id = productId,
                Name = "Test Product",
                PictureUrl = "/images/products/test.jpg"
            };

            _mockProductRepository
                .Setup(r => r.GetByIdAsync(productId))
                .ReturnsAsync(product);

            _mockProductRepository
                .Setup(r => r.Delete(product))
                .Verifiable();

            _mockUnitOfWork
                .Setup(uw => uw.SaveChangeAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _sut.DeleteProduct(productId);

            // Assert
            result.Should().BeTrue();
            _mockProductRepository.Verify(r => r.Delete(product), Times.Once);
            _mockUnitOfWork.Verify(uw => uw.SaveChangeAsync(), Times.Once);
            _mockFileService.Verify(fs => fs.DeleteFile(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task DeleteProduct_WithInvalidId_ThrowsGenericNotFoundException()
        {
            // Arrange
            var invalidId = 999;
            _mockProductRepository
                .Setup(r => r.GetByIdAsync(invalidId))
                .ReturnsAsync((Product)null);

            // Act
            var act = async () => await _sut.DeleteProduct(invalidId);

            // Assert
            await act.Should().ThrowAsync<GenericNotFoundException<Product, int>>();
        }

        [Fact]
        public async Task DeleteProduct_WithProductWithoutPictureUrl_DeletesSuccessfully()
        {
            // Arrange
            var productId = 5;
            var product = new Product
            {
                Id = productId,
                Name = "Test Product",
                PictureUrl = null
            };

            _mockProductRepository
                .Setup(r => r.GetByIdAsync(productId))
                .ReturnsAsync(product);

            _mockUnitOfWork
                .Setup(uw => uw.SaveChangeAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _sut.DeleteProduct(productId);

            // Assert
            result.Should().BeTrue();
            _mockFileService.Verify(fs => fs.DeleteFile(It.IsAny<string>()), Times.Never);
        }

        #endregion

        #region UpdateProduct Tests

        [Fact]
        public async Task UpdateProduct_WithValidDto_UpdatesProductSuccessfully()
        {
            // Arrange
            var updateDto = Fixture.Create<UpdateProductDto>();
            updateDto.Id = 5;
            updateDto.ImageFile = null;

            var product = new Product
            {
                Id = 5,
                Name = "Old Name",
                Description = "Old Description",
                Price = 50m
            };

            var validationResult = new FluentValidation.Results.ValidationResult();

            _mockValidator
                .Setup(v => v.ValidateAsync(updateDto, CancellationToken.None))
                .ReturnsAsync(validationResult);

            _mockProductRepository
                .Setup(r => r.GetByIdAsync(5))
                .ReturnsAsync(product);

            _mockUnitOfWork
                .Setup(uw => uw.SaveChangeAsync())
                .ReturnsAsync(1);

            // Act
            await _sut.UpdateProduct(updateDto);

            // Assert
            product.Name.Should().Be(updateDto.Name);
            product.Description.Should().Be(updateDto.Description);
            product.Price.Should().Be(updateDto.Price);

            _mockUnitOfWork.Verify(uw => uw.SaveChangeAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateProduct_WithInvalidDto_ThrowsException()
        {
            // Arrange
            var updateDto = Fixture.Create<UpdateProductDto>();
            var validationErrors = new List<FluentValidation.Results.ValidationFailure>
            {
                new FluentValidation.Results.ValidationFailure("Name", "Name is required")
            };
            var validationResult = new FluentValidation.Results.ValidationResult(validationErrors);

            _mockValidator
                .Setup(v => v.ValidateAsync(updateDto, CancellationToken.None))
                .ReturnsAsync(validationResult);

            // Act
            var act = async () => await _sut.UpdateProduct(updateDto);

            // Assert
            await act.Should().ThrowAsync<Exception>();
        }

        #endregion
    }
}
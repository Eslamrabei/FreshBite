using FluentAssertions;
using FluentValidation;
using Persistence.Validations.ProductsValidations;
using Shared.Dtos.ProductDto;
using Tests.Fixtures;
using Xunit;

namespace Tests.Validators
{
    public class CreatedProductDtoValidatorTests : TestFixture
    {
        private readonly IValidator<CreatedProductDto> _validator;

        public CreatedProductDtoValidatorTests()
        {
            // Register your actual validator here
            _validator = new CreateProductDtoValidations();
        }

        [Fact]
        public async Task Validate_WithValidDto_ReturnsNoErrors()
        {
            // Arrange
            var dto = new CreatedProductDto
            {
                Name = "Test Product",
                Description = "A valid product description",
                Price = 99.99m,
                BrandId = 1,
                TypeId = 1
            };

            // Act
            var result = await _validator.ValidateAsync(dto);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public async Task Validate_WithNullName_ReturnValidationError()
        {
            // Arrange
            var dto = new CreatedProductDto
            {
                Name = null,
                Description = "A valid product description",
                Price = 99.99m,
                BrandId = 1,
                TypeId = 1
            };

            // Act
            var result = await _validator.ValidateAsync(dto);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(dto.Name));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public async Task Validate_WithInvalidPrice_ReturnValidationError(decimal price)
        {
            // Arrange
            var dto = new CreatedProductDto
            {
                Name = "Test Product",
                Description = "Description",
                Price = price,
                BrandId = 1,
                TypeId = 1
            };

            // Act
            var result = await _validator.ValidateAsync(dto);

            // Assert
            result.IsValid.Should().BeFalse();
        }
    }
}
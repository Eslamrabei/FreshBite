using FluentAssertions;
using FluentValidation;
using Persistence.Validations.ProductsValidations;
using Shared.Dtos.ProductDto;
using Tests.Fixtures;
using Xunit;

namespace Tests.Validators
{
    public class CreateProductDtoValidatorTests : TestFixture
    {
        private readonly CreateProductDtoValidations _validator;

        public CreateProductDtoValidatorTests()
        {
            _validator = new CreateProductDtoValidations();
        }

        [Fact]
        public void Validate_WithValidDto_ReturnsSuccess()
        {
            // Arrange
            var createProductDto = new CreatedProductDto
            {
                Name = "Valid Product",
                Description = "Valid Description",
                Price = 99.99m,
                BrandId = 1,
                TypeId = 1,
                PictureUrl = "http://example.com/image.jpg"
            };

            // Act
            var result = _validator.Validate(createProductDto);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("ab")] // Less than minimum length
        public void Validate_WithInvalidName_ReturnsFailed(string name)
        {
            // Arrange
            var createProductDto = new CreatedProductDto
            {
                Name = name,
                Description = "Valid Description",
                Price = 99.99m
            };

            // Act
            var result = _validator.Validate(createProductDto);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Any(e => e.PropertyName == "Name").Should().BeTrue();
        }

        [Fact]
        public void Validate_WithNameExceedingMaxLength_ReturnsFailed()
        {
            // Arrange
            var createProductDto = new CreatedProductDto
            {
                Name = new string('a', 65), // Exceeds max length of 64
                Description = "Valid Description",
                Price = 99.99m
            };

            // Act
            var result = _validator.Validate(createProductDto);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Any(e => e.PropertyName == "Name").Should().BeTrue();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("abcd")] // Less than minimum length
        public void Validate_WithInvalidDescription_ReturnsFailed(string description)
        {
            // Arrange
            var createProductDto = new CreatedProductDto
            {
                Name = "Valid Product",
                Description = description,
                Price = 99.99m
            };

            // Act
            var result = _validator.Validate(createProductDto);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Any(e => e.PropertyName == "Description").Should().BeTrue();
        }

        [Fact]
        public void Validate_WithDescriptionExceedingMaxLength_ReturnsFailed()
        {
            // Arrange
            var createProductDto = new CreatedProductDto
            {
                Name = "Valid Product",
                Description = new string('a', 257), // Exceeds max length of 256
                Price = 99.99m
            };

            // Act
            var result = _validator.Validate(createProductDto);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Any(e => e.PropertyName == "Description").Should().BeTrue();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10.50)]
        [InlineData(100000)] // Exceeds max value
        public void Validate_WithInvalidPrice_ReturnsFailed(decimal price)
        {
            // Arrange
            var createProductDto = new CreatedProductDto
            {
                Name = "Valid Product",
                Description = "Valid Description",
                Price = price
            };

            // Act
            var result = _validator.Validate(createProductDto);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Any(e => e.PropertyName == "Price").Should().BeTrue();
        }

        [Theory]
        [InlineData(0.01)]
        [InlineData(50.50)]
        [InlineData(99999.99)]
        public void Validate_WithValidPrice_ReturnsSuccess(decimal price)
        {
            // Arrange
            var createProductDto = new CreatedProductDto
            {
                Name = "Valid Product",
                Description = "Valid Description",
                Price = price
            };

            // Act
            var result = _validator.Validate(createProductDto);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_WithMultipleInvalidProperties_ReturnsAllErrors()
        {
            // Arrange
            var createProductDto = new CreatedProductDto
            {
                Name = "ab",
                Description = "abcd",
                Price = 0
            };

            // Act
            var result = _validator.Validate(createProductDto);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCountGreaterThan(1);
        }

        [Fact]
        public void Validate_WithBoundaryValues_ReturnsSuccess()
        {
            // Arrange
            var createProductDto = new CreatedProductDto
            {
                Name = "abc", // Minimum length
                Description = "valid", // Minimum length
                Price = 0.01m
            };

            // Act
            var result = _validator.Validate(createProductDto);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_WithMaximumBoundaryValues_ReturnsSuccess()
        {
            // Arrange
            var createProductDto = new CreatedProductDto
            {
                Name = new string('a', 64), // Maximum length
                Description = new string('b', 256), // Maximum length
                Price = 99999.99m
            };

            // Act
            var result = _validator.Validate(createProductDto);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_WithNullProperties_ReturnsFailed()
        {
            // Arrange
            var createProductDto = new CreatedProductDto
            {
                Name = null,
                Description = null,
                Price = 99.99m
            };

            // Act
            var result = _validator.Validate(createProductDto);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCountGreaterThanOrEqualTo(2);
        }
    }
}

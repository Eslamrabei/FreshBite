using FluentAssertions;
using FluentValidation;
using Persistence.Validations.ProductsValidations;
using Shared.Dtos.AiSearch;
using Tests.Fixtures;
using Xunit;

namespace Tests.Validators
{
    public class UpdateProductDtoValidatorTests : TestFixture
    {
        private readonly UpdateProductDtoValidation _validator;

        public UpdateProductDtoValidatorTests()
        {
            _validator = new UpdateProductDtoValidation();
        }

        [Fact]
        public void Validate_WithValidDto_ReturnsSuccess()
        {
            // Arrange
            var updateProductDto = new UpdateProductDto
            {
                Id = 1,
                Name = "Valid Product",
                Description = "Valid Description",
                Price = 99.99m,
                BrandId = 1,
                TypeId = 1,
                PictureUrl = "http://example.com/image.jpg"
            };

            // Act
            var result = _validator.Validate(updateProductDto);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(1000)]
        public void Validate_WithInvalidId_ReturnsFailed(int id)
        {
            // Arrange
            var updateProductDto = new UpdateProductDto
            {
                Id = id,
                Name = "Valid Product",
                Description = "Valid Description",
                Price = 99.99m
            };

            // Act
            var result = _validator.Validate(updateProductDto);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().NotBeEmpty();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("ab")] // Less than minimum length
        public void Validate_WithInvalidName_ReturnsFailed(string name)
        {
            // Arrange
            var updateProductDto = new UpdateProductDto
            {
                Id = 1,
                Name = name,
                Description = "Valid Description",
                Price = 99.99m
            };

            // Act
            var result = _validator.Validate(updateProductDto);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Any(e => e.PropertyName == "Name").Should().BeTrue();
        }

        [Fact]
        public void Validate_WithNameExceedingMaxLength_ReturnsFailed()
        {
            // Arrange
            var updateProductDto = new UpdateProductDto
            {
                Id = 1,
                Name = new string('a', 65), // Exceeds max length of 64
                Description = "Valid Description",
                Price = 99.99m
            };

            // Act
            var result = _validator.Validate(updateProductDto);

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
            var updateProductDto = new UpdateProductDto
            {
                Id = 1,
                Name = "Valid Product",
                Description = description,
                Price = 99.99m
            };

            // Act
            var result = _validator.Validate(updateProductDto);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Any(e => e.PropertyName == "Description").Should().BeTrue();
        }

        [Fact]
        public void Validate_WithDescriptionExceedingMaxLength_ReturnsFailed()
        {
            // Arrange
            var updateProductDto = new UpdateProductDto
            {
                Id = 1,
                Name = "Valid Product",
                Description = new string('a', 257), // Exceeds max length of 256
                Price = 99.99m
            };

            // Act
            var result = _validator.Validate(updateProductDto);

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
            var updateProductDto = new UpdateProductDto
            {
                Id = 1,
                Name = "Valid Product",
                Description = "Valid Description",
                Price = price
            };

            // Act
            var result = _validator.Validate(updateProductDto);

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
            var updateProductDto = new UpdateProductDto
            {
                Id = 1,
                Name = "Valid Product",
                Description = "Valid Description",
                Price = price
            };

            // Act
            var result = _validator.Validate(updateProductDto);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_WithMultipleInvalidProperties_ReturnsAllErrors()
        {
            // Arrange
            var updateProductDto = new UpdateProductDto
            {
                Id = 0,
                Name = "ab",
                Description = "abcd",
                Price = 0
            };

            // Act
            var result = _validator.Validate(updateProductDto);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCountGreaterThan(1);
        }

        [Fact]
        public void Validate_WithBoundaryValues_ReturnsSuccess()
        {
            // Arrange
            var updateProductDto = new UpdateProductDto
            {
                Id = 1,
                Name = "abc", // Minimum length
                Description = "valid", // Minimum length
                Price = 0.01m
            };

            // Act
            var result = _validator.Validate(updateProductDto);

            // Assert
            result.IsValid.Should().BeTrue();
        }
    }
}

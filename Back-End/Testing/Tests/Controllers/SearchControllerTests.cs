using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Presentation.Controllers;
using ServiceAbstraction.Contracts;
using Shared.Dtos.AiSearch;
using Tests.Fixtures;
using Xunit;

namespace Tests.Controllers
{
    public class SearchControllerTests : TestFixture
    {
        private readonly SearchController _sut;
        private readonly Mock<IEmbeddingService> _mockEmbeddingService;
        private readonly Mock<IVectorService> _mockVectorService;
        private readonly Mock<IOllamaService> _mockOllama;
        private readonly Mock<IGroqService> _mockGroq;

        public SearchControllerTests()
        {
            _mockEmbeddingService = MockOf<IEmbeddingService>();
            _mockVectorService = MockOf<IVectorService>();
            _mockOllama = MockOf<IOllamaService>();
            _mockGroq = MockOf<IGroqService>();

            _sut = new SearchController(
                _mockEmbeddingService.Object,
                _mockVectorService.Object,
                _mockOllama.Object,
                _mockGroq.Object
            );
        }

        [Fact]
        public async Task Search_WithValidQuery_ReturnsRagResponse()
        {
            // Arrange
            var query = "fresh tomatoes";
            var queryVector = new float[] { 0.1f, 0.2f, 0.3f };
            var searchResults = new List<ProductSearchResponse>
            {
                new() { Id = 1, Name = "Tomato", Description = "Fresh red tomato", Price = 10.50m, Score = 0.9f }
            };
            var aiResponse = "We have fresh tomatoes available!";

            _mockEmbeddingService.Setup(e => e.GetEmbeddingAsync(query))
                .ReturnsAsync(queryVector);

            _mockVectorService.Setup(v => v.SearchAsync(queryVector, null))
                .ReturnsAsync(searchResults);

            _mockGroq.Setup(g => g.GenerateRagResponseAsync(query, searchResults))
                .ReturnsAsync(aiResponse);

            // Act
            var result = await _sut.Search(query);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult?.Value.Should().BeOfType<RagResponseDto>();
            var ragDto = okResult?.Value as RagResponseDto;
            ragDto?.AiAnswer.Should().Be(aiResponse);
            ragDto?.Products.Should().HaveCount(1);
        }

        [Fact]
        public async Task Search_WithNullQuery_ReturnsBadRequest()
        {
            // Act
            var result = await _sut.Search(null);

            // Assert
            result.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task Search_WithEmptyQuery_ReturnsBadRequest()
        {
            // Act
            var result = await _sut.Search(string.Empty);

            // Assert
            result.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task Search_WithWhitespaceQuery_ReturnsBadRequest()
        {
            // Act
            var result = await _sut.Search("   ");

            // Assert
            result.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task Search_WithPriceFilter_PassesPriceToVectorService()
        {
            // Arrange
            var query = "affordable products";
            var maxPrice = 50m;
            var queryVector = new float[] { 0.4f, 0.5f, 0.6f };
            var searchResults = new List<ProductSearchResponse>();

            _mockEmbeddingService.Setup(e => e.GetEmbeddingAsync(query))
                .ReturnsAsync(queryVector);

            _mockVectorService.Setup(v => v.SearchAsync(queryVector, maxPrice))
                .ReturnsAsync(searchResults);

            _mockGroq.Setup(g => g.GenerateRagResponseAsync(It.IsAny<string>(), It.IsAny<List<ProductSearchResponse>>()))
                .ReturnsAsync("No products found within budget.");

            // Act
            var result = await _sut.Search(query, maxPrice);

            // Assert
            _mockVectorService.Verify(v => v.SearchAsync(queryVector, maxPrice), Times.Once);
        }

        [Fact]
        public async Task Search_WithNoResults_ReturnsDefaultMessage()
        {
            // Arrange
            var query = "nonexistent product";
            var queryVector = new float[] { 0.7f, 0.8f, 0.9f };
            var emptyResults = new List<ProductSearchResponse>();
            var defaultMessage = "I'm sorry, I couldn't find any products matching your criteria.";

            _mockEmbeddingService.Setup(e => e.GetEmbeddingAsync(query))
                .ReturnsAsync(queryVector);

            _mockVectorService.Setup(v => v.SearchAsync(queryVector, null))
                .ReturnsAsync(emptyResults);

            // Act
            var result = await _sut.Search(query);

            // Assert
            var okResult = result as OkObjectResult;
            var ragDto = okResult?.Value as RagResponseDto;
            ragDto?.AiAnswer.Should().Be(defaultMessage);
            _mockGroq.Verify(g => g.GenerateRagResponseAsync(It.IsAny<string>(), It.IsAny<List<ProductSearchResponse>>()), Times.Never);
        }

        [Fact]
        public async Task Search_WithMultipleResults_IncludesAllInResponse()
        {
            // Arrange
            var query = "vegetables";
            var queryVector = new float[] { 0.2f, 0.3f, 0.4f };
            var searchResults = new List<ProductSearchResponse>
            {
                new() { Id = 1, Name = "Carrot", Description = "Orange carrot", Price = 5.00m, Score = 0.95f },
                new() { Id = 2, Name = "Broccoli", Description = "Green broccoli", Price = 12.00m, Score = 0.92f },
                new() { Id = 3, Name = "Lettuce", Description = "Crispy lettuce", Price = 8.00m, Score = 0.88f }
            };

            _mockEmbeddingService.Setup(e => e.GetEmbeddingAsync(query))
                .ReturnsAsync(queryVector);

            _mockVectorService.Setup(v => v.SearchAsync(queryVector, null))
                .ReturnsAsync(searchResults);

            _mockGroq.Setup(g => g.GenerateRagResponseAsync(query, searchResults))
                .ReturnsAsync("Great selection of vegetables available!");

            // Act
            var result = await _sut.Search(query);

            // Assert
            var okResult = result as OkObjectResult;
            var ragDto = okResult?.Value as RagResponseDto;
            ragDto?.Products.Should().HaveCount(3);
        }

        [Fact]
        public async Task GetRecommendations_WithValidProductId_ReturnsRecommendedProducts()
        {
            // Arrange
            var productId = 1;
            var recommendations = new List<ProductSearchResponse>
            {
                new() { Id = 2, Name = "Similar Product 1", Description = "Similar to product 1", Price = 50.00m, Score = 0.92f },
                new() { Id = 3, Name = "Similar Product 2", Description = "Also similar to product 1", Price = 55.00m, Score = 0.89f }
            };

            _mockVectorService.Setup(v => v.GetRecommendationAsync(productId))
                .ReturnsAsync(recommendations);

            // Act
            var result = await _sut.GetRecommendations(productId);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var returnedRecs = okResult?.Value as List<ProductSearchResponse>;
            returnedRecs.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetRecommendations_WithNonExistentProduct_ReturnsNotFound()
        {
            // Arrange
            var productId = 99999;
            var emptyRecommendations = new List<ProductSearchResponse>();

            _mockVectorService.Setup(v => v.GetRecommendationAsync(productId))
                .ReturnsAsync(emptyRecommendations);

            // Act
            var result = await _sut.GetRecommendations(productId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
            var notFoundResult = result as NotFoundObjectResult;
            notFoundResult?.Value.Should().Be("Product not found or no recommendations available.");
        }

        [Fact]
        public async Task GetRecommendations_WithSingleRecommendation_ReturnsOneProduct()
        {
            // Arrange
            var productId = 5;
            var recommendations = new List<ProductSearchResponse>
            {
                new() { Id = 6, Name = "Recommended Product", Description = "Recommended for you", Price = 75.00m, Score = 0.95f }
            };

            _mockVectorService.Setup(v => v.GetRecommendationAsync(productId))
                .ReturnsAsync(recommendations);

            // Act
            var result = await _sut.GetRecommendations(productId);

            // Assert
            var okResult = result as OkObjectResult;
            var returnedRecs = okResult?.Value as List<ProductSearchResponse>;
            returnedRecs.Should().HaveCount(1);
            returnedRecs?.First().Id.Should().Be(6);
        }

        [Fact]
        public async Task Search_CallsEmbeddingServiceWithQuery()
        {
            // Arrange
            var query = "test query";
            var queryVector = new float[] { 0.1f, 0.2f };

            _mockEmbeddingService.Setup(e => e.GetEmbeddingAsync(query))
                .ReturnsAsync(queryVector);

            _mockVectorService.Setup(v => v.SearchAsync(It.IsAny<float[]>(), null))
                .ReturnsAsync(new List<ProductSearchResponse>());

            _mockGroq.Setup(g => g.GenerateRagResponseAsync(It.IsAny<string>(), It.IsAny<List<ProductSearchResponse>>()))
                .ReturnsAsync("Response");

            // Act
            await _sut.Search(query);

            // Assert
            _mockEmbeddingService.Verify(e => e.GetEmbeddingAsync(query), Times.Once);
        }
    }
}

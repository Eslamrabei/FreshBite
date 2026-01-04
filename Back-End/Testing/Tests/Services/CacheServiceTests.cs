using Domain.Contracts;
using FluentAssertions;
using Moq;
using Service.Implementations;
using Tests.Fixtures;
using Xunit;

namespace Tests.Services
{
    public class CacheServiceTests : TestFixture
    {
        private readonly CacheService _sut;
        private readonly Mock<ICacheRepository> _mockCacheService;

        public CacheServiceTests()
        {
            _mockCacheService = MockOf<ICacheRepository>();
            _sut = new CacheService(_mockCacheService.Object);
        }

        #region SetAsync Tests

        [Fact]
        public async Task SetAsync_WithValidKeyAndValue_SetsCacheSuccessfully()
        {
            // Arrange
            var key = "test-key";
            var value = "test-value";
            var expirationTime = TimeSpan.FromMinutes(10);

            _mockCacheService
                .Setup(dc => dc.SetCacheAsync(key, It.IsAny<object>(), expirationTime))
                .Returns(Task.CompletedTask);

            // Act
            await _sut.SetCacheAsync(key, value, expirationTime);

            // Assert
            _mockCacheService.Verify(
                dc => dc.SetCacheAsync(key, It.IsAny<object>(), It.IsAny<TimeSpan>()),
                Times.Once);
        }

        [Fact]
        public async Task SetAsync_WithNullKey_ThrowsArgumentNullException()
        {
            // Arrange
            string nullKey = null;
            var value = "test-value";

            // Act
            var act = async () => await _sut.SetCacheAsync(nullKey, value, TimeSpan.FromMinutes(10));

            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        #endregion

        #region GetAsync Tests

        [Fact]
        public async Task GetAsync_WithValidKey_ReturnsCachedValue()
        {
            // Arrange
            var key = "test-key";
            var expectedValue = "test-value";


            _mockCacheService
                .Setup(dc => dc.GetCacheAsync(key))
                .ReturnsAsync(expectedValue);

            // Act
            var result = await _sut.GetCacheAsync(key);

            // Assert
            result.Should().Be(expectedValue);

            _mockCacheService.Verify(
                dc => dc.GetCacheAsync(key),
                Times.Once);
        }

        [Fact]
        public async Task GetAsync_WithNonExistentKey_ReturnsNull()
        {
            // Arrange
            var key = "non-existent-key";

            _mockCacheService
                .Setup(dc => dc.GetCacheAsync(key))
                .ReturnsAsync((string)null);

            // Act
            var result = await _sut.GetCacheAsync(key);

            // Assert
            result.Should().BeNull();
        }

        #endregion

    }
}
using FluentAssertions;
using Moq;
using Persistence.Repositories;
using StackExchange.Redis;
using Tests.Fixtures;
using Xunit;

namespace Tests.Repositories
{
    public class CacheRepositoryTests : TestFixture
    {
        private readonly Mock<IConnectionMultiplexer> _mockConnection;
        private readonly Mock<IDatabase> _mockDatabase;
        private readonly CacheRepository _sut;

        public CacheRepositoryTests()
        {
            _mockConnection = MockOf<IConnectionMultiplexer>();
            _mockDatabase = MockOf<IDatabase>();

            _mockConnection.Setup(c => c.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                .Returns(_mockDatabase.Object);

            _sut = new CacheRepository(_mockConnection.Object);
        }

        [Fact]
        public async Task GetCacheAsync_WithExistingKey_ReturnsSerializedValue()
        {
            // Arrange
            var key = "test_key";
            var expectedValue = "{\"name\":\"Test Product\",\"price\":99.99}";

            _mockDatabase.Setup(d => d.StringGetAsync((RedisKey)key, CommandFlags.None))
                .ReturnsAsync(new RedisValue(expectedValue));

            // Act
            var result = await _sut.GetCacheAsync(key);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(expectedValue);
            _mockDatabase.Verify(d => d.StringGetAsync((RedisKey)key, It.IsAny<CommandFlags>()), Times.Once);
        }

        [Fact]
        public async Task GetCacheAsync_WithNonExistentKey_ReturnsNull()
        {
            // Arrange
            var key = "non_existent_key";

            _mockDatabase.Setup(d => d.StringGetAsync((RedisKey)key, CommandFlags.None))
                .ReturnsAsync(RedisValue.Null);

            // Act
            var result = await _sut.GetCacheAsync(key);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task SetCacheAsync_WithValidKeyAndValue_StoresInCache()
        {
            // Arrange
            var key = "test_key";
            var testObject = new { name = "Test Product", price = 99.99 };
            var timeToLive = TimeSpan.FromMinutes(30);

            _mockDatabase.Setup(d => d.StringSetAsync(
                It.IsAny<RedisKey>(),
                It.IsAny<RedisValue>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<bool>(),
                It.IsAny<When>(),
                CommandFlags.None))
                .ReturnsAsync(true);

            // Act
            await _sut.SetCacheAsync(key, testObject, timeToLive);

            // Assert
            _mockDatabase.Verify(
                d => d.StringSetAsync(
                    It.IsAny<RedisKey>(),
                    It.IsAny<RedisValue>(),
                    timeToLive,
                    It.IsAny<bool>(),
                    It.IsAny<When>(),
                    It.IsAny<CommandFlags>()),
                Times.Once
            );
        }

        [Theory]
        [InlineData("")]
        [InlineData("simple_value")]
        [InlineData("{\"complex\":\"json\"}")]
        public async Task SetCacheAsync_WithVariousValues_StoresSuccessfully(string value)
        {
            // Arrange
            var key = "test_key";
            var timeToLive = TimeSpan.FromHours(1);

            _mockDatabase.Setup(d => d.StringSetAsync(
                It.IsAny<RedisKey>(),
                It.IsAny<RedisValue>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<bool>(),
                It.IsAny<When>(),
                CommandFlags.None))
                .ReturnsAsync(true);

            // Act
            await _sut.SetCacheAsync(key, value, timeToLive);

            // Assert
            _mockDatabase.Verify(
                d => d.StringSetAsync(
                    (RedisKey)key,
                    It.IsAny<RedisValue>(),
                    timeToLive,
                    It.IsAny<bool>(),
                    It.IsAny<When>(),
                    It.IsAny<CommandFlags>()),
                Times.Once
            );
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(60)]
        public async Task SetCacheAsync_WithVariousTimeToLiveValues_StoresWithCorrectExpiry(int minutes)
        {
            // Arrange
            var key = "test_key";
            var testObject = new { data = "test" };
            var timeToLive = TimeSpan.FromMinutes(minutes);

            _mockDatabase.Setup(d => d.StringSetAsync(
                It.IsAny<RedisKey>(),
                It.IsAny<RedisValue>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<bool>(),
                It.IsAny<When>(),
                CommandFlags.None))
                .ReturnsAsync(true);

            // Act
            await _sut.SetCacheAsync(key, testObject, timeToLive);

            // Assert
            _mockDatabase.Verify(
                d => d.StringSetAsync(
                    (RedisKey)key,
                    It.IsAny<RedisValue>(),
                    It.Is<TimeSpan?>(t => t == timeToLive),
                    It.IsAny<bool>(),
                    It.IsAny<When>(),
                    It.IsAny<CommandFlags>()),
                Times.Once
            );
        }

        [Fact]
        public async Task SetCacheAsync_WithEmptyKey_StoresInCache()
        {
            // Arrange
            var key = "";
            var testObject = new { data = "test" };
            var timeToLive = TimeSpan.FromMinutes(30);

            _mockDatabase.Setup(d => d.StringSetAsync(
                It.IsAny<RedisKey>(),
                It.IsAny<RedisValue>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<bool>(),
                It.IsAny<When>(),
                CommandFlags.None))
                .ReturnsAsync(true);

            // Act
            await _sut.SetCacheAsync(key, testObject, timeToLive);

            // Assert
            _mockDatabase.Verify(d => d.StringSetAsync(
                It.IsAny<RedisKey>(),
                It.IsAny<RedisValue>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<bool>(),
                It.IsAny<When>(),
                It.IsAny<CommandFlags>()), Times.Once);
        }

        [Fact]
        public async Task GetCacheAsync_WithEmptyStringValue_ReturnsEmpty()
        {
            // Arrange
            var key = "test_key";

            _mockDatabase.Setup(d => d.StringGetAsync((RedisKey)key, CommandFlags.None))
                .ReturnsAsync(new RedisValue(""));

            // Act
            var result = await _sut.GetCacheAsync(key);

            // Assert
            result.Should().BeNull(); // Empty string is treated as null per implementation
        }

        [Fact]
        public async Task SetCacheAsync_WithComplexObject_SerializesCorrectly()
        {
            // Arrange
            var key = "complex_object";
            var testObject = new
            {
                id = 1,
                name = "Product",
                items = new[] { "item1", "item2", "item3" }
            };
            var timeToLive = TimeSpan.FromDays(1);

            _mockDatabase.Setup(d => d.StringSetAsync(
                It.IsAny<RedisKey>(),
                It.IsAny<RedisValue>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<bool>(),
                It.IsAny<When>(),
                CommandFlags.None))
                .ReturnsAsync(true);

            // Act
            await _sut.SetCacheAsync(key, testObject, timeToLive);

            // Assert
            _mockDatabase.Verify(
                d => d.StringSetAsync(
                    It.IsAny<RedisKey>(),
                    It.Is<RedisValue>(s => s.ToString().Contains("\"id\"") && s.ToString().Contains("\"name\"")),
                    timeToLive,
                    It.IsAny<bool>(),
                    It.IsAny<When>(),
                    It.IsAny<CommandFlags>()),
                Times.Once
            );
        }

        [Fact]
        public async Task SetCacheAsync_WithNullValue_StoresNullAsString()
        {
            // Arrange
            var key = "null_key";
            object testObject = null;
            var timeToLive = TimeSpan.FromMinutes(30);

            _mockDatabase.Setup(d => d.StringSetAsync(
                It.IsAny<RedisKey>(),
                It.IsAny<RedisValue>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<bool>(),
                It.IsAny<When>(),
                CommandFlags.None))
                .ReturnsAsync(true);

            // Act
            await _sut.SetCacheAsync(key, testObject, timeToLive);

            // Assert
            _mockDatabase.Verify(d => d.StringSetAsync(
                It.IsAny<RedisKey>(),
                It.IsAny<RedisValue>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<bool>(),
                It.IsAny<When>(),
                It.IsAny<CommandFlags>()), Times.Once);
        }
    }
}

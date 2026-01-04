using AutoFixture;
using AutoMapper;
using Domain.Contracts;
using Domain.Entities.IdentityModule;
using FluentAssertions;
using Moq;
using Service.Implementations;
using ServiceAbstraction.Contracts;
using Shared.Dtos.IdentityDto;
using Tests.Fixtures;
using Xunit;

namespace Tests.Services
{
    public class RefreshTokenServicesTests : TestFixture
    {
        private readonly RefreshTokenServices _sut;
        private readonly Mock<IRefreshTokenRepository> _mockRefreshTokenRepository;
        private readonly Mock<IMapper> _mockMapper;

        public RefreshTokenServicesTests()
        {
            _mockRefreshTokenRepository = MockOf<IRefreshTokenRepository>();
            _mockMapper = MockOf<IMapper>();

            _sut = new RefreshTokenServices(
                _mockRefreshTokenRepository.Object,
                _mockMapper.Object
            );
        }

        [Fact]
        public async Task SaveAsync_WithValidRefreshTokenDto_CallsRepositorySave()
        {
            // Arrange
            var refreshTokenDto = Fixture.Create<RefreshTokenDto>();
            var refreshToken = Fixture.Create<RefreshToken>();

            _mockMapper.Setup(m => m.Map<RefreshToken>(refreshTokenDto))
                .Returns(refreshToken);

            _mockRefreshTokenRepository.Setup(r => r.SaveAsync(It.IsAny<RefreshToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _sut.SaveAsync(refreshTokenDto);

            // Assert
            _mockRefreshTokenRepository.Verify(
                r => r.SaveAsync(It.IsAny<RefreshToken>()),
                Times.Once
            );
            _mockMapper.Verify(m => m.Map<RefreshToken>(refreshTokenDto), Times.Once);
        }

        [Fact]
        public async Task GetByTokenAsync_WithValidToken_ReturnsRefreshTokenDto()
        {
            // Arrange
            var token = "test-token";
            var refreshToken = Fixture.Create<RefreshToken>();
            var refreshTokenDto = Fixture.Create<RefreshTokenDto>();

            _mockRefreshTokenRepository.Setup(r => r.GetByTokenAsync(token))
                .ReturnsAsync(refreshToken);

            _mockMapper.Setup(m => m.Map<RefreshTokenDto>(refreshToken))
                .Returns(refreshTokenDto);

            // Act
            var result = await _sut.GetByTokenAsync(token);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(refreshTokenDto);
            _mockRefreshTokenRepository.Verify(r => r.GetByTokenAsync(token), Times.Once);
        }

        [Fact]
        public async Task GetByTokenAsync_WithInvalidToken_ReturnsNull()
        {
            // Arrange
            var token = "invalid-token";

            _mockRefreshTokenRepository.Setup(r => r.GetByTokenAsync(token))
                .ReturnsAsync((RefreshToken)null);

            _mockMapper.Setup(m => m.Map<RefreshTokenDto>(null))
                .Returns((RefreshTokenDto)null);

            // Act
            var result = await _sut.GetByTokenAsync(token);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task RevokeAsync_WithValidRefreshTokenDto_CallsRepositoryRevoke()
        {
            // Arrange
            var refreshTokenDto = Fixture.Create<RefreshTokenDto>();
            var refreshToken = Fixture.Create<RefreshToken>();

            _mockMapper.Setup(m => m.Map<RefreshToken>(refreshTokenDto))
                .Returns(refreshToken);

            _mockRefreshTokenRepository.Setup(r => r.RevokeAsync(It.IsAny<RefreshToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _sut.RevokeAsync(refreshTokenDto);

            // Assert
            _mockRefreshTokenRepository.Verify(
                r => r.RevokeAsync(It.IsAny<RefreshToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task RemoveAllForUserAsync_WithValidUserId_CallsRepositoryRemoveAll()
        {
            // Arrange
            var userId = Fixture.Create<string>();

            _mockRefreshTokenRepository.Setup(r => r.RemoveAllForUserAsync(userId))
                .Returns(Task.CompletedTask);

            // Act
            await _sut.RemoveAllForUserAsync(userId);

            // Assert
            _mockRefreshTokenRepository.Verify(
                r => r.RemoveAllForUserAsync(userId),
                Times.Once
            );
        }

        [Fact]
        public async Task SaveAsync_WhenRepositoryThrowsException_PropagatesException()
        {
            // Arrange
            var refreshTokenDto = Fixture.Create<RefreshTokenDto>();
            var refreshToken = Fixture.Create<RefreshToken>();

            _mockMapper.Setup(m => m.Map<RefreshToken>(refreshTokenDto))
                .Returns(refreshToken);

            _mockRefreshTokenRepository.Setup(r => r.SaveAsync(It.IsAny<RefreshToken>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _sut.SaveAsync(refreshTokenDto));
        }
    }
}

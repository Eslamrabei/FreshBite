using AutoFixture;
using AutoMapper;
using Domain.Contracts;
using Domain.Entities.IdentityModule;
using Domain.Exceptions;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using Service.Implementations;
using ServiceAbstraction.Contracts;
using Shared.Common;
using Shared.Dtos.IdentityDto;
using Tests.Fixtures;
using Xunit;

namespace Tests.Services
{
    public class AuthenticationServiceTests : TestFixture
    {
        private readonly AuthenticationService _sut;
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly Mock<IOptions<JwtOptions>> _mockJwtOptions;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IRefreshTokenServices> _mockRefreshTokenServices;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IRefreshTokenRepository> _mockRefreshTokenRepository;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<RoleManager<IdentityRole>> _mockRoleMAnager;

        public AuthenticationServiceTests()
        {
            _mockUserManager = CreateMockUserManager();
            _mockJwtOptions = MockOf<IOptions<JwtOptions>>();
            _mockMapper = MockOf<IMapper>();
            _mockRefreshTokenServices = MockOf<IRefreshTokenServices>();
            _mockUnitOfWork = MockOf<IUnitOfWork>();
            _mockRefreshTokenRepository = MockOf<IRefreshTokenRepository>();
            _mockConfiguration = MockOf<IConfiguration>();
            _mockRoleMAnager = new();


            _sut = new AuthenticationService(
                _mockUserManager.Object,
                _mockJwtOptions.Object,
                _mockMapper.Object,
                _mockRefreshTokenServices.Object,
                _mockUnitOfWork.Object,
                _mockRefreshTokenRepository.Object,
                _mockConfiguration.Object,
                _mockRoleMAnager.Object
            );
        }

        #region Helper Methods

        private Mock<UserManager<User>> CreateMockUserManager()
        {
            var store = MockOf<IUserStore<User>>();
            var mgr = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);
            mgr.Object.UserValidators.Add(new UserValidator<User>());
            mgr.Object.PasswordValidators.Add(new PasswordValidator<User>());
            return mgr;
        }

        #endregion

        #region CheckEmailExistAsync Tests

        [Fact]
        public async Task CheckEmailExistAsync_WithExistingEmail_ReturnsTrue()
        {
            // Arrange
            var email = "test@example.com";
            var user = Fixture.Create<User>();

            _mockUserManager
                .Setup(um => um.FindByEmailAsync(email))
                .ReturnsAsync(user);

            // Act
            var result = await _sut.CheckEmailExistAsync(email);

            // Assert
            result.Should().BeTrue();

            _mockUserManager.Verify(um => um.FindByEmailAsync(email), Times.Once);
        }

        [Fact]
        public async Task CheckEmailExistAsync_WithNonExistentEmail_ReturnsFalse()
        {
            // Arrange
            var email = "nonexistent@example.com";

            _mockUserManager
                .Setup(um => um.FindByEmailAsync(email))
                .ReturnsAsync((User)null);

            // Act
            var result = await _sut.CheckEmailExistAsync(email);

            // Assert
            result.Should().BeFalse();
        }

        #endregion

        #region GetCurrentUserAsync Tests

        [Fact]
        public async Task GetCurrentUserAsync_WithValidEmail_ReturnsUserResultDto()
        {
            // Arrange
            var userEmail = "test@example.com";
            var user = new User { Email = userEmail, DisplayName = "Test User" };
            var expectedDto = new UserResultDto("Test User", "access-token", "refresh-token", userEmail);

            _mockUserManager
                .Setup(um => um.FindByEmailAsync(userEmail))
                .ReturnsAsync(user);

            _mockJwtOptions
                .Setup(opt => opt.Value)
                .Returns(Fixture.Create<JwtOptions>());

            _mockUserManager.Setup(u => u.GetRolesAsync(user))
                .ReturnsAsync(["user"]);

            // Act
            var result = await _sut.GetCurrentUSerAsync(userEmail);

            // Assert
            result.Should().NotBeNull();
            result.Email.Should().Be(userEmail);

            _mockUserManager.Verify(um => um.FindByEmailAsync(userEmail), Times.Once);

        }

        [Fact]
        public async Task GetCurrentUserAsync_WithInvalidEmail_ThrowsGenericNotFoundException()
        {
            // Arrange
            var invalidEmail = "nonexistent@example.com";

            _mockUserManager
                .Setup(um => um.FindByEmailAsync(invalidEmail))
                .ReturnsAsync((User)null);

            // Act
            var act = async () => await _sut.GetCurrentUSerAsync(invalidEmail);

            // Assert
            await act.Should().ThrowAsync<GenericNotFoundException<User, int>>();
        }

        #endregion

        #region LoginAsync Tests

        [Fact]
        public async Task LoginAsync_WithValidCredentials_ReturnsUserResultDto()
        {
            // Arrange
            var loginDto = new LoginDto { Email = "test@example.com", Password = "Password123!" };
            var user = new User { Email = loginDto.Email, DisplayName = "Test User" };

            _mockUserManager
                .Setup(um => um.FindByEmailAsync(loginDto.Email))
                .ReturnsAsync(user);

            _mockUserManager
                .Setup(um => um.CheckPasswordAsync(user, loginDto.Password))
                .ReturnsAsync(true);

            _mockJwtOptions
                .Setup(opt => opt.Value)
                .Returns(Fixture.Create<JwtOptions>());

            _mockRefreshTokenServices
                .Setup(rts => rts.SaveAsync(It.IsAny<RefreshTokenDto>()))
                .Returns(Task.CompletedTask);

            _mockMapper
                .Setup(m => m.Map<RefreshTokenDto>(It.IsAny<RefreshToken>()))
                .Returns(Fixture.Create<RefreshTokenDto>());

            _mockUserManager
                .Setup(u => u.GetRolesAsync(user)).ReturnsAsync(["user"]);

            // Act
            var result = await _sut.LoginAsync(loginDto);

            // Assert
            result.Should().NotBeNull();
            result.Email.Should().Be(loginDto.Email);
            result.AccessToken.Should().NotBeNullOrEmpty();
            result.RefreshToken.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task LoginAsync_WithInvalidEmail_ThrowsUnauthorizeException()
        {
            // Arrange
            var loginDto = new LoginDto { Email = "nonexistent@example.com", Password = "Password123!" };

            _mockUserManager
                .Setup(um => um.FindByEmailAsync(loginDto.Email))
                .ReturnsAsync((User)null);

            // Act
            var act = async () => await _sut.LoginAsync(loginDto);

            // Assert
            await act.Should().ThrowAsync<UnauthorizeException>();
        }

        [Fact]
        public async Task LoginAsync_WithInvalidPassword_ThrowsUnauthorizeException()
        {
            // Arrange
            var loginDto = new LoginDto { Email = "test@example.com", Password = "WrongPassword" };
            var user = Fixture.Create<User>();

            _mockUserManager
                .Setup(um => um.FindByEmailAsync(loginDto.Email))
                .ReturnsAsync(user);

            _mockUserManager
                .Setup(um => um.CheckPasswordAsync(user, loginDto.Password))
                .ReturnsAsync(false);

            // Act
            var act = async () => await _sut.LoginAsync(loginDto);

            // Assert
            await act.Should().ThrowAsync<UnauthorizeException>();
        }

        #endregion
    }
}
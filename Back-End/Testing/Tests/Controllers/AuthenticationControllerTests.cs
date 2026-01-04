using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Presentation.Controllers;
using ServiceAbstraction.Contracts;
using Shared.Dtos.IdentityDto;
using Tests.Fixtures;
using Xunit;

namespace Tests.Controllers
{
    public class AuthenticationControllerTests : TestFixture
    {
        private readonly AuthenticationController _sut;
        private readonly Mock<IServiceManager> _mockServiceManager;

        public AuthenticationControllerTests()
        {
            _mockServiceManager = MockOf<IServiceManager>();
            _sut = new AuthenticationController(_mockServiceManager.Object);
        }

        #region RegisterAsync Tests

        [Fact]
        public async Task RegisterAsync_WithValidRegisterDto_ReturnsOkWithUserResult()
        {
            // Arrange
            var registerDto = Fixture.Create<RegisterDto>();
            var expectedUserResult = Fixture.Create<UserEmailConfirmRegisteration>();

            _mockServiceManager
                .Setup(sm => sm.AuthenticationService.RegisterAsync(registerDto))
                .ReturnsAsync(expectedUserResult);

            // Act
            var result = await _sut.RegisterAsync(registerDto);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();

            var okResult = result.Result as OkObjectResult;
            okResult.Value
                .Should()
                .BeEquivalentTo(expectedUserResult);

            _mockServiceManager.Verify(
                sm => sm.AuthenticationService.RegisterAsync(registerDto),
                Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_WithNullDto_ThrowsException()
        {
            // Arrange
            RegisterDto nullDto = null;

            _mockServiceManager
                .Setup(sm => sm.AuthenticationService.RegisterAsync(nullDto))
                .ThrowsAsync(new ArgumentNullException(nameof(RegisterDto)));

            // Act
            var act = async () => await _sut.RegisterAsync(nullDto);

            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        #endregion

        #region LoginAsync Tests

        [Fact]
        public async Task LoginAsync_WithValidLoginDto_ReturnsOkWithUserResult()
        {
            // Arrange
            var loginDto = Fixture.Create<LoginDto>();
            var expectedUserResult = Fixture.Create<UserResultDto>();

            _mockServiceManager
                .Setup(sm => sm.AuthenticationService.LoginAsync(loginDto))
                .ReturnsAsync(expectedUserResult);

            // Act
            var result = await _sut.LoginAsync(loginDto);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();

            var okResult = result.Result as OkObjectResult;
            okResult.Value
                .Should()
                .BeEquivalentTo(expectedUserResult);

            _mockServiceManager.Verify(
                sm => sm.AuthenticationService.LoginAsync(loginDto),
                Times.Once);
        }

        #endregion

        #region CheckEmailExistAsync Tests

        [Fact]
        public async Task CheckEmailExistAsync_WithExistingEmail_ReturnsTrue()
        {
            // Arrange
            var email = "test@example.com";

            _mockServiceManager
                .Setup(sm => sm.AuthenticationService.CheckEmailExistAsync(email))
                .ReturnsAsync(true);

            // Act
            var result = await _sut.CheckEmailExistAsync(email);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();

            var okResult = result.Result as OkObjectResult;
            okResult.Value.Should().Be(true);
        }

        #endregion
    }
}
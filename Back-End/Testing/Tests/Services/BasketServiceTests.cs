using AutoFixture;
using AutoMapper;
using Domain.Contracts;
using Domain.Entities.BasketModule;
using Domain.Exceptions;
using FluentAssertions;
using Moq;
using Service.Implementations;
using Shared.Dtos.BasketDto;
using Tests.Fixtures;
using Xunit;

namespace Tests.Services
{
    public class BasketServiceTests : TestFixture
    {
        private readonly BasketService _sut;
        private readonly Mock<IBasketRepository> _mockBasketRepository;
        private readonly Mock<IMapper> _mockMapper;

        public BasketServiceTests()
        {
            _mockBasketRepository = MockOf<IBasketRepository>();
            _mockMapper = MockOf<IMapper>();

            _sut = new BasketService(
                _mockBasketRepository.Object,
                _mockMapper.Object
            );
        }

        #region GetBasketAsync Tests

        [Fact]
        public async Task GetBasketAsync_WithValidId_ReturnsBasketDto()
        {
            // Arrange
            var basketId = "test-basket-id";
            var customerBasket = new CustomerBasket
            {
                Id = basketId,
                Items = Fixture.CreateMany<BasketItems>(3).ToList(),
                ShippingPrice = 10m
            };
            var expectedBasketDto = Fixture.Create<BasketDto>();

            _mockBasketRepository
                .Setup(br => br.GetBasketAsync(basketId))
                .ReturnsAsync(customerBasket);

            _mockMapper
                .Setup(m => m.Map<BasketDto>(customerBasket))
                .Returns(expectedBasketDto);

            // Act
            var result = await _sut.GetBasketAsync(basketId);

            // Assert
            result.Should().BeEquivalentTo(expectedBasketDto);

            _mockBasketRepository.Verify(br => br.GetBasketAsync(basketId), Times.Once);
            _mockMapper.Verify(m => m.Map<BasketDto>(customerBasket), Times.Once);
        }

        [Fact]
        public async Task GetBasketAsync_WithInvalidId_ThrowsGenericNotFoundException()
        {
            // Arrange
            var invalidId = "non-existent-id";
            _mockBasketRepository
                .Setup(br => br.GetBasketAsync(invalidId))
                .ReturnsAsync((CustomerBasket)null);

            // Act
            var act = async () => await _sut.GetBasketAsync(invalidId);

            // Assert
            await act.Should().ThrowAsync<GenericNotFoundException<CustomerBasket, int>>();
        }

        #endregion

        #region CreateOrUpdateAsync Tests

        [Fact]
        public async Task CreateOrUpdateAsync_WithValidBasketDto_ReturnsUpdatedBasketDto()
        {
            // Arrange
            var basketDto = Fixture.Create<BasketDto>();
            var customerBasket = Fixture.Create<CustomerBasket>();
            customerBasket.Id = "test-id";
            var updatedBasketDto = Fixture.Create<BasketDto>();

            _mockMapper
                .Setup(m => m.Map<CustomerBasket>(basketDto))
                .Returns(customerBasket);

            _mockBasketRepository
                .Setup(br => br.CreateOrUpdateAsync(customerBasket, null))
                .ReturnsAsync(customerBasket);

            _mockBasketRepository
                .Setup(br => br.GetBasketAsync(customerBasket.Id))
                .ReturnsAsync(customerBasket);

            _mockMapper
                .Setup(m => m.Map<BasketDto>(customerBasket))
                .Returns(updatedBasketDto);

            // Act
            var result = await _sut.CreateOrUpdateAsync(basketDto);

            // Assert
            result.Should().BeEquivalentTo(updatedBasketDto);

            _mockBasketRepository.Verify(br => br.CreateOrUpdateAsync(customerBasket, null), Times.Once);
            _mockBasketRepository.Verify(br => br.GetBasketAsync(customerBasket.Id), Times.Once);
        }

        [Fact]
        public async Task CreateOrUpdateAsync_WhenCreateReturnNull_ThrowsException()
        {
            // Arrange
            var basketDto = Fixture.Create<BasketDto>();
            var customerBasket = Fixture.Create<CustomerBasket>();

            _mockMapper
                .Setup(m => m.Map<CustomerBasket>(basketDto))
                .Returns(customerBasket);

            _mockBasketRepository
                .Setup(br => br.CreateOrUpdateAsync(customerBasket, null))
                .ReturnsAsync((CustomerBasket)null);

            // Act
            var act = async () => await _sut.CreateOrUpdateAsync(basketDto);

            // Assert
            await act.Should().ThrowAsync<GenericNotFoundException<CustomerBasket, int>>();
        }

        #endregion

        #region DeleteBasketAsync Tests

        [Fact]
        public async Task DeleteBasketAsync_WithValidId_ReturnsTrue()
        {
            // Arrange
            var basketId = "test-basket-id";
            _mockBasketRepository
                .Setup(br => br.DaleteAsync(basketId))
                .ReturnsAsync(true);

            // Act
            var result = await _sut.DeleteBasketAsync(basketId);

            // Assert
            result.Should().BeTrue();

            _mockBasketRepository.Verify(br => br.DaleteAsync(basketId), Times.Once);
        }

        [Fact]
        public async Task DeleteBasketAsync_WithInvalidId_ReturnsFalse()
        {
            // Arrange
            var invalidId = "non-existent-id";
            _mockBasketRepository
                .Setup(br => br.DaleteAsync(invalidId))
                .ReturnsAsync(false);

            // Act
            var result = await _sut.DeleteBasketAsync(invalidId);

            // Assert
            result.Should().BeFalse();
        }

        #endregion
    }
}
using AutoFixture;
using AutoMapper;
using Domain.Contracts;
using Domain.Entities.BasketModule;
using Domain.Entities.OrderModule;
using Domain.Exceptions;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Service.Implementations;
using Shared.Dtos.BasketDto;
using Stripe;
using Tests.Fixtures;
using Xunit;
using Product = Domain.Entities.ProductModule.Product;

namespace Tests.Services
{
    public class PaymentServiceTests : TestFixture
    {
        private readonly PaymentService _sut;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IBasketRepository> _mockBasketRepository;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<PaymentIntentService> _mockPayments;
        public PaymentServiceTests()
        {
            _mockConfiguration = MockOf<IConfiguration>();
            _mockBasketRepository = MockOf<IBasketRepository>();
            _mockUnitOfWork = MockOf<IUnitOfWork>();
            _mockMapper = MockOf<IMapper>();
            _mockPayments = new();

            _sut = new PaymentService(
                _mockConfiguration.Object,
                _mockBasketRepository.Object,
                _mockUnitOfWork.Object,
                _mockMapper.Object,
                _mockPayments.Object
            );
        }

        #region CreateOrUpdatePaymentIntentIdAsync Tests

        [Fact]
        public async Task CreateOrUpdatePaymentIntentIdAsync_WithValidBasketId_ReturnsBasketDto()
        {
            // Arrange
            var basketId = "test-basket-id";
            var customerBasket = new CustomerBasket
            {
                Id = basketId,
                Items = new List<BasketItems>
                {
                    new BasketItems { Id = 1, Quantity = 2, Price = 50m }
                },
                ShippingPrice = 10m,
                DeliveryMethodId = 1,
                PaymentIndentId = "test-payment",
                ClientSecret = "test-client-secret"
            };
            var deliveryMethod = new DeliveryMethod
            {
                Id = 1,
                Price = 10m
            };
            long Total = 1023;
            var Options = new PaymentIntentUpdateOptions
            {
                Amount = Total
            };


            var product = new Domain.Entities.ProductModule.Product { Id = 1, Name = "Test Product", Price = 50m };
            var expectedBasketDto = Fixture.Create<BasketDto>();

            var mockProductRepo = MockOf<IGenericRepository<Domain.Entities.ProductModule.Product, int>>();
            mockProductRepo.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(product);

            _mockUnitOfWork
                .Setup(u => u.GetRepository<DeliveryMethod, int>().GetByIdAsync(customerBasket.DeliveryMethodId.Value))
                .ReturnsAsync(deliveryMethod);

            _mockConfiguration
                .Setup(c => c["StripSettings:SecretKey"])
                .Returns("test-secret-key");

            _mockBasketRepository
                .Setup(br => br.GetBasketAsync(basketId))
                .ReturnsAsync(customerBasket);

            _mockUnitOfWork
                .Setup(uw => uw.GetRepository<Product, int>())
                .Returns(mockProductRepo.Object);

            _mockPayments
                .Setup(p => p.UpdateAsync(customerBasket.PaymentIndentId,
                It.Is<PaymentIntentUpdateOptions>(o => o.Amount == Total),
                null, default))
                .ReturnsAsync(new PaymentIntent
                {
                    Id = "pi_test_123",
                    Amount = Total,
                    Currency = "usd",
                    Status = "requires_payment_method"
                });


            _mockBasketRepository
                .Setup(br => br.CreateOrUpdateAsync(customerBasket, null))
                .ReturnsAsync(customerBasket);


            _mockMapper
                .Setup(m => m.Map<BasketDto>(customerBasket))
                .Returns(expectedBasketDto);

            // Act
            var result = await _sut.CreateOrUpdatePaymentIntentIdAsync(basketId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedBasketDto);

            _mockBasketRepository.Verify(br => br.GetBasketAsync(basketId), Times.Once);
            _mockBasketRepository.Verify(br => br.CreateOrUpdateAsync(customerBasket, null), Times.Once);
        }

        [Fact]
        public async Task CreateOrUpdatePaymentIntentIdAsync_WithInvalidBasketId_ThrowsGenericNotFoundException()
        {
            // Arrange
            var basketId = "test-basket-id";
            var customerBasket = new CustomerBasket
            {
                Id = basketId,
                Items = new List<BasketItems>
                {
                    new BasketItems { Id = 999, Quantity = 2, Price = 50m }
                },
                ShippingPrice = 10m
            };

            var mockProductRepo = MockOf<IGenericRepository<Product, int>>();
            mockProductRepo.Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((Product)null);

            _mockConfiguration
                .Setup(c => c["StripSettings:SecretKey"])
                .Returns("test-secret-key");

            _mockBasketRepository
                .Setup(br => br.GetBasketAsync(basketId))
                .ReturnsAsync(customerBasket);

            _mockUnitOfWork
                .Setup(uw => uw.GetRepository<Product, int>())
                .Returns(mockProductRepo.Object);

            // Act
            var act = async () => await _sut.CreateOrUpdatePaymentIntentIdAsync(basketId);

            // Assert
            await act.Should().ThrowAsync<GenericNotFoundException<Product, int>>();
        }

        #endregion
    }
}
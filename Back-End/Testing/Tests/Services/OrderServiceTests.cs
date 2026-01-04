using AutoFixture;
using AutoMapper;
using Domain.Contracts;
using Domain.Entities.BasketModule;
using Domain.Entities.OrderModule;
using Domain.Entities.ProductModule;
using Domain.Exceptions;
using FluentAssertions;
using Moq;
using Service.Implementations;
using Service.Specifications;
using Shared.Dtos.OrderDto;
using Tests.Fixtures;
using Xunit;

namespace Tests.Services
{
    public class OrderServiceTests : TestFixture
    {
        private readonly OrderService _sut;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IBasketRepository> _mockBasketRepository;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;

        public OrderServiceTests()
        {
            _mockMapper = MockOf<IMapper>();
            _mockBasketRepository = MockOf<IBasketRepository>();
            _mockUnitOfWork = MockOf<IUnitOfWork>();

            _sut = new OrderService(
                _mockMapper.Object,
                _mockBasketRepository.Object,
                _mockUnitOfWork.Object
            );
        }

        #region CreateOrderAsync Tests

        [Fact]
        public async Task CreateOrderAsync_WithValidOrderRequest_CreatesAndReturnsOrder()
        {
            // Arrange
            var userEmail = "test@example.com";
            var basketId = "test-basket-id";
            var deliveryMethodId = 1;

            var address = Fixture.Create<AddressDto>();
            var orderRequest = new OrderRequest
            {
                BasketId = basketId,
                DeliveryMethodId = deliveryMethodId,
                ShipToAddress = address
            };

            var basketItem = new BasketItems { Id = 1, Quantity = 2, Price = 50m };
            var customerBasket = new CustomerBasket
            {
                Id = basketId,
                Items = new List<BasketItems> { basketItem },
                PaymentIndentId = "test-payment-id"
            };

            var product = new Product { Id = 1, Name = "Test Product", Price = 50m, PictureUrl = "/images/test.jpg" };
            var deliveryMethod = new DeliveryMethod { Id = deliveryMethodId, ShortName = "Standard", Price = 10m };

            var expectedOrderDto = Fixture.Create<OrderResultDto>();

            // Setup mocks
            _mockBasketRepository
                .Setup(br => br.GetBasketAsync(basketId))
                .ReturnsAsync(customerBasket);

            var mockProductRepo = MockOf<IGenericRepository<Product, int>>();
            mockProductRepo.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(product);

            var mockOrderRepo = MockOf<IGenericRepository<Order, Guid>>();
            mockOrderRepo.Setup(r => r.GetByIdAsync(It.IsAny<OrderWithPaymentInetntIdSpecifications>()))
                .ReturnsAsync((Order)null);
            mockOrderRepo.Setup(r => r.AddAsync(It.IsAny<Order>()))
                .Returns(Task.CompletedTask);

            var mockDeliveryRepo = MockOf<IGenericRepository<DeliveryMethod, int>>();
            mockDeliveryRepo.Setup(r => r.GetByIdAsync(deliveryMethodId))
                .ReturnsAsync(deliveryMethod);

            _mockUnitOfWork
                .Setup(uw => uw.GetRepository<Product, int>())
                .Returns(mockProductRepo.Object);

            _mockUnitOfWork
                .Setup(uw => uw.GetRepository<Order, Guid>())
                .Returns(mockOrderRepo.Object);

            _mockUnitOfWork
                .Setup(uw => uw.GetRepository<DeliveryMethod, int>())
                .Returns(mockDeliveryRepo.Object);

            _mockUnitOfWork
                .Setup(uw => uw.SaveChangeAsync())
                .ReturnsAsync(1);

            _mockMapper
                .Setup(m => m.Map<Address>(address))
                .Returns(Fixture.Create<Address>());

            _mockMapper
                .Setup(m => m.Map<OrderResultDto>(It.IsAny<Order>()))
                .Returns(expectedOrderDto);

            // Act
            var result = await _sut.CreateOrderAsync(orderRequest, userEmail);

            // Assert
            result.Should().BeEquivalentTo(expectedOrderDto);

            mockOrderRepo.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Once);
            _mockUnitOfWork.Verify(uw => uw.SaveChangeAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateOrderAsync_WithInvalidBasketId_ThrowsGenericNotFoundException()
        {
            // Arrange
            var userEmail = "test@example.com";
            var orderRequest = new OrderRequest
            {
                BasketId = "non-existent-id",
                DeliveryMethodId = 1,
                ShipToAddress = Fixture.Create<AddressDto>()
            };

            _mockBasketRepository
                .Setup(br => br.GetBasketAsync("non-existent-id"))
                .ReturnsAsync((CustomerBasket)null);

            // Act
            var act = async () => await _sut.CreateOrderAsync(orderRequest, userEmail);

            // Assert
            await act.Should().ThrowAsync<GenericNotFoundException<CustomerBasket, int>>();
        }

        #endregion

        #region GetDeliverMethodAsync Tests

        [Fact]
        public async Task GetDeliverMethodAsync_ReturnsAllDeliveryMethods()
        {
            // Arrange
            var deliveryMethods = Fixture.CreateMany<DeliveryMethod>(3).ToList();
            var deliveryMethodDtos = Fixture.CreateMany<DeliverMethodResult>(3).ToList();

            var mockRepo = MockOf<IGenericRepository<DeliveryMethod, int>>();
            mockRepo.Setup(r => r.GetAllAsync(false)).ReturnsAsync(deliveryMethods);

            _mockUnitOfWork
                .Setup(uw => uw.GetRepository<DeliveryMethod, int>())
                .Returns(mockRepo.Object);

            _mockMapper
                .Setup(m => m.Map<IEnumerable<DeliverMethodResult>>(deliveryMethods))
                .Returns(deliveryMethodDtos);

            // Act
            var result = await _sut.GetDeliverMethodAsync();

            // Assert
            result.Should().HaveCount(3);
            result.Should().BeEquivalentTo(deliveryMethodDtos);

            mockRepo.Verify(r => r.GetAllAsync(false), Times.Once);
        }

        #endregion

        #region GetAllOrdersByEmailAsync Tests

        [Fact]
        public async Task GetAllOrdersByEmailAsync_WithValidEmail_ReturnsUserOrders()
        {
            // Arrange
            var userEmail = "test@example.com";
            var orders = Fixture.CreateMany<Order>(2).ToList();
            var orderDtos = Fixture.CreateMany<OrderResultDto>(2).ToList();

            var mockRepo = MockOf<IGenericRepository<Order, Guid>>();
            mockRepo.Setup(r => r.GetAllAsync(It.IsAny<OrderWithIncludesSpecifications>()))
                .ReturnsAsync(orders);

            _mockUnitOfWork
                .Setup(uw => uw.GetRepository<Order, Guid>())
                .Returns(mockRepo.Object);

            _mockMapper
                .Setup(m => m.Map<IEnumerable<OrderResultDto>>(orders))
                .Returns(orderDtos);

            // Act
            var result = await _sut.GetAllOrdersByEmailAsync(userEmail);

            // Assert
            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(orderDtos);

            mockRepo.Verify(
                r => r.GetAllAsync(It.IsAny<OrderWithIncludesSpecifications>()),
                Times.Once);
        }

        #endregion

        #region GetOrderByIdAsync Tests

        [Fact]
        public async Task GetOrderByIdAsync_WithValidId_ReturnsOrder()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = Fixture.Create<Order>();
            var orderDto = Fixture.Create<OrderResultDto>();

            var mockRepo = MockOf<IGenericRepository<Order, Guid>>();
            mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<OrderWithIncludesSpecifications>()))
                .ReturnsAsync(order);

            _mockUnitOfWork
                .Setup(uw => uw.GetRepository<Order, Guid>())
                .Returns(mockRepo.Object);

            _mockMapper
                .Setup(m => m.Map<OrderResultDto>(order))
                .Returns(orderDto);

            // Act
            var result = await _sut.GetOrderByIdAsync(orderId);

            // Assert
            result.Should().BeEquivalentTo(orderDto);

            mockRepo.Verify(
                r => r.GetByIdAsync(It.IsAny<OrderWithIncludesSpecifications>()),
                Times.Once);
        }

        [Fact]
        public async Task GetOrderByIdAsync_WithInvalidId_ThrowsGenericNotFoundException()
        {
            // Arrange
            var invalidId = Guid.NewGuid();
            var mockRepo = MockOf<IGenericRepository<Order, Guid>>();
            mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<OrderWithIncludesSpecifications>()))
                .ReturnsAsync((Order)null);

            _mockUnitOfWork
                .Setup(uw => uw.GetRepository<Order, Guid>())
                .Returns(mockRepo.Object);

            // Act
            var act = async () => await _sut.GetOrderByIdAsync(invalidId);

            // Assert
            await act.Should().ThrowAsync<GenericNotFoundException<Order, Guid>>();
        }

        #endregion
    }
}
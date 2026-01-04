using AutoFixture;
using Shared.Dtos.IdentityDto;
using Shared.Dtos.ProductDto;

namespace Tests.Helpers
{
    public class TestDataBuilder
    {
        private readonly IFixture _fixture;

        public TestDataBuilder()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.Add(new AutoFixture.OmitOnRecursionBehavior());
        }

        public CreatedProductDto BuildValidProduct()
        {
            return new CreatedProductDto
            {
                Name = "Test Product",
                Description = "Test Description",
                Price = 99.99m,
                BrandId = 1,
                TypeId = 1,
                PictureUrl = "http://example.com/image.jpg"
            };
        }

        public RegisterDto BuildValidRegisterDto()
        {
            return new RegisterDto
            {
                Email = $"test{Guid.NewGuid()}@example.com",
                Password = "TestPassword123!",
                DisplayName = "Test User",
                PhoneNumber = "1234567890"
            };
        }

        public LoginDto BuildValidLoginDto()
        {
            return new LoginDto
            {
                Email = "test@example.com",
                Password = "TestPassword123!"
            };
        }

        public T Build<T>() where T : class
        {
            return _fixture.Create<T>();
        }

        public IEnumerable<T> BuildMany<T>(int count) where T : class
        {
            return _fixture.CreateMany<T>(count);
        }
    }
}
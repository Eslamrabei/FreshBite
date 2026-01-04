using AutoFixture;
using Moq;

namespace Tests.Fixtures
{
    public class TestFixture
    {
        protected readonly IFixture Fixture;

        public TestFixture()
        {
            Fixture = new Fixture();
            Fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        protected Mock<T> MockOf<T>() where T : class
        {
            return new Mock<T>();
        }
    }
}
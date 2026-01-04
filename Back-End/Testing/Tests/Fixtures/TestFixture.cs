using AutoFixture;
using AutoFixture.Kernel;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Tests.Fixtures
{
    public class TestFixture
    {
        protected readonly IFixture Fixture;

        public TestFixture()
        {
            Fixture = new Fixture();
            // Configure a custom specimen builder for IFormFile
            Fixture.Customizations.Add(
                new FormFileBuilder());

            Fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        }

        protected Mock<T> MockOf<T>() where T : class
        {
            return new Mock<T>();
        }
    }

    /// <summary>
    /// Custom builder for IFormFile that creates FormFile instances with a MemoryStream.
    /// </summary>
    public class FormFileBuilder : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            var type = request as Type;

            if (type != null && (type == typeof(IFormFile) || type == typeof(FormFile)))
            {
                using var stream = new MemoryStream();
                return new FormFile(stream, 0, stream.Length, "file", "testfile.txt");
            }

            return new NoSpecimen();
        }
    }
}
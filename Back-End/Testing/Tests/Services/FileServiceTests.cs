using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Moq;
using Service.Implementations;
using Tests.Fixtures;
using Xunit;

namespace Tests.Services
{
    public class FileServiceTests : TestFixture
    {
        private readonly FileService _sut;
        private readonly Mock<IWebHostEnvironment> _mockWebHostEnvironment;

        public FileServiceTests()
        {
            _mockWebHostEnvironment = MockOf<IWebHostEnvironment>();
            _sut = new FileService(_mockWebHostEnvironment.Object);
        }

        #region UploadFileAsync Tests

        [Fact]
        public async Task UploadFileAsync_WithValidPngFile_ReturnsFilePath()
        {
            // Arrange
            var webRootPath = Path.Combine(Path.GetTempPath(), "wwwroot");
            Directory.CreateDirectory(webRootPath);

            _mockWebHostEnvironment.Setup(e => e.WebRootPath).Returns(webRootPath);

            var fileContent = "test content";
            var fileName = "test.png";
            var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(fileContent));
            var formFile = new FormFile(stream, 0, stream.Length, "file", fileName);

            // Act
            var result = await _sut.UploadFileAsync(formFile, "products");

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().StartWith("/images/products");
            result.Should().EndWith(".png");

            // Cleanup
            if (Directory.Exists(webRootPath))
                Directory.Delete(webRootPath, true);
        }

        [Theory]
        [InlineData("test.doc")]
        [InlineData("test.txt")]
        [InlineData("test.exe")]
        public async Task UploadFileAsync_WithInvalidExtension_ReturnsNull(string fileName)
        {
            // Arrange
            var webRootPath = Path.Combine(Path.GetTempPath(), "wwwroot");
            _mockWebHostEnvironment.Setup(e => e.WebRootPath).Returns(webRootPath);

            var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("content"));
            var formFile = new FormFile(stream, 0, stream.Length, "file", fileName);

            // Act
            var result = await _sut.UploadFileAsync(formFile, "products");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task UploadFileAsync_WithFileExceedingMaxSize_ReturnsNull()
        {
            // Arrange
            var webRootPath = Path.Combine(Path.GetTempPath(), "wwwroot");
            _mockWebHostEnvironment.Setup(e => e.WebRootPath).Returns(webRootPath);

            var largeContent = new byte[3_000_000]; // > 2MB
            var stream = new MemoryStream(largeContent);
            var formFile = new FormFile(stream, 0, stream.Length, "file", "large.png");

            // Act
            var result = await _sut.UploadFileAsync(formFile, "products");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task UploadFileAsync_WithEmptyFile_ReturnsNull()
        {
            // Arrange
            var webRootPath = Path.Combine(Path.GetTempPath(), "wwwroot");
            _mockWebHostEnvironment.Setup(e => e.WebRootPath).Returns(webRootPath);

            var stream = new MemoryStream();
            var formFile = new FormFile(stream, 0, 0, "file", "empty.png");

            // Act
            var result = await _sut.UploadFileAsync(formFile, "products");

            // Assert
            result.Should().BeNull();
        }

        #endregion

        #region DeleteFile Tests

        [Fact]
        public void DeleteFile_WithValidFilePath_DeletesFile()
        {
            // Arrange
            var tempFilePath = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid()}.txt");
            File.WriteAllText(tempFilePath, "test content");

            // Act
            _sut.DeleteFile(tempFilePath);

            // Assert
            File.Exists(tempFilePath).Should().BeFalse();
        }

        [Fact]
        public void DeleteFile_WithNullPath_DoesNotThrow()
        {
            // Act
            var act = () => _sut.DeleteFile(null);

            // Assert
            act.Should().NotThrow();
        }

        [Fact]
        public void DeleteFile_WithNonExistentFilePath_DoesNotThrow()
        {
            // Act
            var act = () => _sut.DeleteFile("non/existent/path.txt");

            // Assert
            act.Should().NotThrow();
        }

        #endregion
    }
}
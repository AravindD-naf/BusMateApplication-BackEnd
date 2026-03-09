using BusTicketingSystem.DTOs.Requests;
using BusTicketingSystem.DTOs.Responses;
using BusTicketingSystem.Exceptions;
using BusTicketingSystem.Models;
using BusTicketingSystem.Repositories;
using BusTicketingSystem.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace BusTicketingSystem.Tests.Services
{
    public class SourceServiceTests
    {
        private readonly Mock<ISourceRepository> _mockSourceRepository;
        private readonly SourceService _sourceService;

        public SourceServiceTests()
        {
            _mockSourceRepository = new Mock<ISourceRepository>();
            _sourceService = new SourceService(_mockSourceRepository);
        }

        [Fact]
        public async Task CreateSourceAsync_WithValidRequest_ShouldCreateSource()
        {
            // Arrange
            var request = new CreateSourceRequest
            {
                SourceName = "Delhi",
                Description = "Capital of India"
            };

            _mockSourceRepository.Setup(x => x.CreateAsync(It.IsAny<Source>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _sourceService.CreateSourceAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.SourceName.Should().Be("Delhi");
            result.IsActive.Should().BeTrue();
            _mockSourceRepository.Verify(x => x.CreateAsync(It.IsAny<Source>()), Times.Once);
        }

        [Fact]
        public async Task CreateSourceAsync_WithEmptySourceName_ShouldThrowBadRequestException()
        {
            // Arrange
            var request = new CreateSourceRequest
            {
                SourceName = "",
                Description = "Description"
            };

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(
                () => _sourceService.CreateSourceAsync(request));
        }

        [Fact]
        public async Task GetAllSourcesAsync_WithValidPagination_ShouldReturnSources()
        {
            // Arrange
            var sources = new List<Source>
            {
                new Source { SourceId = 1, SourceName = "Delhi", IsActive = true },
                new Source { SourceId = 2, SourceName = "Mumbai", IsActive = true }
            };

            _mockSourceRepository.Setup(x => x.GetAllAsync(1, 10))
                .ReturnsAsync(sources);

            // Act
            var result = await _sourceService.GetAllSourcesAsync(1, 10);

            // Assert
            result.Should().HaveCount(2);
            result.Should().AllSatisfy(s => s.IsActive.Should().BeTrue());
        }

        [Fact]
        public async Task GetSourceByIdAsync_WithInvalidId_ShouldThrowNotFoundException()
        {
            // Arrange
            _mockSourceRepository.Setup(x => x.GetByIdAsync(999))
                .ReturnsAsync((Source)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(
                () => _sourceService.GetSourceByIdAsync(999));
        }

        [Fact]
        public async Task UpdateSourceAsync_WithValidRequest_ShouldUpdateSource()
        {
            // Arrange
            var source = new Source { SourceId = 1, SourceName = "Delhi", IsActive = true };
            var request = new UpdateSourceRequest
            {
                SourceName = "New Delhi",
                Description = "Updated Description",
                IsActive = true
            };

            _mockSourceRepository.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(source);

            _mockSourceRepository.Setup(x => x.UpdateAsync(It.IsAny<Source>()))
                .Returns(Task.CompletedTask);

            // Act
            await _sourceService.UpdateSourceAsync(1, request);

            // Assert
            _mockSourceRepository.Verify(x => x.UpdateAsync(It.IsAny<Source>()), Times.Once);
        }

        [Fact]
        public async Task DeleteSourceAsync_WithValidId_ShouldDeleteSource()
        {
            // Arrange
            var source = new Source { SourceId = 1, SourceName = "Delhi", IsActive = true };

            _mockSourceRepository.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(source);

            _mockSourceRepository.Setup(x => x.DeleteAsync(1))
                .Returns(Task.CompletedTask);

            // Act
            await _sourceService.DeleteSourceAsync(1);

            // Assert
            _mockSourceRepository.Verify(x => x.DeleteAsync(1), Times.Once);
        }
    }
}

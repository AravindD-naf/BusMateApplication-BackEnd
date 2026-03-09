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
    public class DestinationServiceTests
    {
        private readonly Mock<IDestinationRepository> _mockDestinationRepository;
        private readonly DestinationService _destinationService;

        public DestinationServiceTests()
        {
            _mockDestinationRepository = new Mock<IDestinationRepository>();
            _destinationService = new DestinationService(_mockDestinationRepository);
        }

        [Fact]
        public async Task CreateDestinationAsync_WithValidRequest_ShouldCreateDestination()
        {
            // Arrange
            var request = new CreateDestinationRequest
            {
                DestinationName = "Bangalore",
                Description = "Tech City"
            };

            _mockDestinationRepository.Setup(x => x.CreateAsync(It.IsAny<Destination>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _destinationService.CreateDestinationAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.DestinationName.Should().Be("Bangalore");
            result.IsActive.Should().BeTrue();
            _mockDestinationRepository.Verify(x => x.CreateAsync(It.IsAny<Destination>()), Times.Once);
        }

        [Fact]
        public async Task CreateDestinationAsync_WithEmptyDestinationName_ShouldThrowBadRequestException()
        {
            // Arrange
            var request = new CreateDestinationRequest
            {
                DestinationName = "",
                Description = "Description"
            };

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(
                () => _destinationService.CreateDestinationAsync(request));
        }

        [Fact]
        public async Task GetAllDestinationsAsync_WithValidPagination_ShouldReturnDestinations()
        {
            // Arrange
            var destinations = new List<Destination>
            {
                new Destination { DestinationId = 1, DestinationName = "Bangalore", IsActive = true },
                new Destination { DestinationId = 2, DestinationName = "Chennai", IsActive = true }
            };

            _mockDestinationRepository.Setup(x => x.GetAllAsync(1, 10))
                .ReturnsAsync(destinations);

            // Act
            var result = await _destinationService.GetAllDestinationsAsync(1, 10);

            // Assert
            result.Should().HaveCount(2);
            result.Should().AllSatisfy(d => d.IsActive.Should().BeTrue());
        }

        [Fact]
        public async Task GetDestinationByIdAsync_WithInvalidId_ShouldThrowNotFoundException()
        {
            // Arrange
            _mockDestinationRepository.Setup(x => x.GetByIdAsync(999))
                .ReturnsAsync((Destination)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(
                () => _destinationService.GetDestinationByIdAsync(999));
        }

        [Fact]
        public async Task UpdateDestinationAsync_WithValidRequest_ShouldUpdateDestination()
        {
            // Arrange
            var destination = new Destination { DestinationId = 1, DestinationName = "Bangalore", IsActive = true };
            var request = new UpdateDestinationRequest
            {
                DestinationName = "Bengaluru",
                Description = "Updated Tech City",
                IsActive = true
            };

            _mockDestinationRepository.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(destination);

            _mockDestinationRepository.Setup(x => x.UpdateAsync(It.IsAny<Destination>()))
                .Returns(Task.CompletedTask);

            // Act
            await _destinationService.UpdateDestinationAsync(1, request);

            // Assert
            _mockDestinationRepository.Verify(x => x.UpdateAsync(It.IsAny<Destination>()), Times.Once);
        }

        [Fact]
        public async Task DeleteDestinationAsync_WithValidId_ShouldDeleteDestination()
        {
            // Arrange
            var destination = new Destination { DestinationId = 1, DestinationName = "Bangalore", IsActive = true };

            _mockDestinationRepository.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(destination);

            _mockDestinationRepository.Setup(x => x.DeleteAsync(1))
                .Returns(Task.CompletedTask);

            // Act
            await _destinationService.DeleteDestinationAsync(1);

            // Assert
            _mockDestinationRepository.Verify(x => x.DeleteAsync(1), Times.Once);
        }
    }
}

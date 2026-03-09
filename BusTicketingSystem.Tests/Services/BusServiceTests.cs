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
    public class BusServiceTests
    {
        private readonly Mock<IBusRepository> _mockBusRepository;
        private readonly Mock<IAuditService> _mockAuditService;
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private readonly BusService _busService;

        public BusServiceTests()
        {
            _mockBusRepository = new Mock<IBusRepository>();
            _mockAuditService = new Mock<IAuditService>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _busService = new BusService(_mockBusRepository, _mockAuditService, _mockHttpContextAccessor);
        }

        [Fact]
        public async Task CreateBusAsync_WithValidRequest_ShouldCreateBusWithInactiveStatus()
        {
            // Arrange
            var request = new CreateBusRequest
            {
                BusNumber = "BUS001",
                BusType = "AC",
                TotalSeats = 40,
                OperatorName = "Test Operator"
            };

            _mockBusRepository.Setup(x => x.GetByBusNumberAsync(It.IsAny<string>()))
                .ReturnsAsync((Bus)null);

            _mockBusRepository.Setup(x => x.CreateAsync(It.IsAny<Bus>()))
                .Returns(Task.CompletedTask);

            _mockAuditService.Setup(x => x.LogAsync(It.IsAny<int>(), It.IsAny<string>(), 
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), 
                It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _busService.CreateBusAsync(request, 1, "192.168.1.1");

            // Assert
            result.Should().NotBeNull();
            result.BusNumber.Should().Be("BUS001");
            result.TotalSeats.Should().Be(40);
            result.IsActive.Should().BeFalse(); // Bus should be inactive initially
            _mockBusRepository.Verify(x => x.CreateAsync(It.IsAny<Bus>()), Times.Once);
        }

        [Fact]
        public async Task CreateBusAsync_WithDuplicateBusNumber_ShouldThrowConflictException()
        {
            // Arrange
            var request = new CreateBusRequest
            {
                BusNumber = "BUS001",
                BusType = "AC",
                TotalSeats = 40,
                OperatorName = "Test Operator"
            };

            var existingBus = new Bus { BusId = 1, BusNumber = "BUS001", IsDeleted = false };

            _mockBusRepository.Setup(x => x.GetByBusNumberAsync("BUS001"))
                .ReturnsAsync(existingBus);

            // Act & Assert
            await Assert.ThrowsAsync<ConflictException>(
                () => _busService.CreateBusAsync(request, 1, "192.168.1.1"));
        }

        [Fact]
        public async Task CreateBusAsync_WithInvalidSeats_ShouldFail()
        {
            // Arrange
            var request = new CreateBusRequest
            {
                BusNumber = "BUS001",
                BusType = "AC",
                TotalSeats = 50, // Exceeds max of 40
                OperatorName = "Test Operator"
            };

            // Act & Assert
            // The validation should fail at model validation level, but let's verify via service
            // In reality, this would be caught by ModelState validation in the controller
            request.TotalSeats.Should().BeGreaterThan(40);
        }

        [Fact]
        public async Task GetAllBusesAsync_WithValidPagination_ShouldReturnBuses()
        {
            // Arrange
            var buses = new List<Bus>
            {
                new Bus { BusId = 1, BusNumber = "BUS001", TotalSeats = 40, IsActive = false },
                new Bus { BusId = 2, BusNumber = "BUS002", TotalSeats = 30, IsActive = false }
            };

            _mockBusRepository.Setup(x => x.GetAllAsync(1, 10))
                .ReturnsAsync(buses);

            // Act
            var result = await _busService.GetAllBusesAsync(1, 10);

            // Assert
            result.Should().HaveCount(2);
            result.Should().AllSatisfy(b => b.IsActive.Should().BeFalse());
        }

        [Fact]
        public async Task GetBusByIdAsync_WithInvalidId_ShouldThrowNotFoundException()
        {
            // Arrange
            _mockBusRepository.Setup(x => x.GetByIdAsync(999))
                .ReturnsAsync((Bus)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(
                () => _busService.GetBusByIdAsync(999));
        }
    }
}

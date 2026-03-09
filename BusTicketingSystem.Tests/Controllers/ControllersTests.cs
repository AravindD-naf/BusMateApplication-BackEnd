using BusTicketingSystem.Controllers;
using BusTicketingSystem.DTOs.Requests;
using BusTicketingSystem.DTOs.Responses;
using BusTicketingSystem.Helpers;
using BusTicketingSystem.Interfaces.Services;
using BusTicketingSystem.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace BusTicketingSystem.Tests.Controllers
{
    public class BusesControllerTests
    {
        private readonly Mock<IBusService> _mockBusService;
        private readonly BusesController _controller;

        public BusesControllerTests()
        {
            _mockBusService = new Mock<IBusService>();
            _controller = new BusesController(_mockBusService);
        }

        [Fact]
        public async Task CreateBus_WithValidRequest_ShouldReturnOkResult()
        {
            // Arrange
            var request = new CreateBusRequest
            {
                BusNumber = "BUS001",
                BusType = "AC",
                TotalSeats = 40,
                OperatorName = "TestOperator"
            };

            var expectedResponse = new BusResponse
            {
                BusId = 1,
                BusNumber = "BUS001",
                BusType = "AC",
                TotalSeats = 40,
                OperatorName = "TestOperator",
                IsActive = false
            };

            _mockBusService.Setup(x => x.CreateBusAsync(It.IsAny<CreateBusRequest>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.CreateBus(request);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GetAllBuses_WithValidPagination_ShouldReturnBuses()
        {
            // Arrange
            var buses = new List<BusResponse>
            {
                new BusResponse { BusId = 1, BusNumber = "BUS001", TotalSeats = 40, IsActive = false },
                new BusResponse { BusId = 2, BusNumber = "BUS002", TotalSeats = 30, IsActive = false }
            };

            var request = new PaginationRequest { PageNumber = 1, PageSize = 10 };

            _mockBusService.Setup(x => x.GetAllBusesAsync(1, 10))
                .ReturnsAsync(buses);

            // Act
            var result = await _controller.GetAllBuses(request);

            // Assert
            result.Should().NotBeNull();
            _mockBusService.Verify(x => x.GetAllBusesAsync(1, 10), Times.Once);
        }

        [Fact]
        public async Task GetBus_WithValidId_ShouldReturnBus()
        {
            // Arrange
            var busResponse = new BusResponse
            {
                BusId = 1,
                BusNumber = "BUS001",
                BusType = "AC",
                TotalSeats = 40,
                OperatorName = "TestOperator",
                IsActive = false
            };

            _mockBusService.Setup(x => x.GetBusByIdAsync(1))
                .ReturnsAsync(busResponse);

            // Act
            var result = await _controller.GetBus(1);

            // Assert
            result.Should().NotBeNull();
            _mockBusService.Verify(x => x.GetBusByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetByOperator_WithValidOperatorName_ShouldReturnBuses()
        {
            // Arrange
            var request = new OperatorSearchRequest
            {
                OperatorName = "TestOperator",
                PageNumber = 1,
                PageSize = 10
            };

            var buses = new List<BusResponse>
            {
                new BusResponse { BusId = 1, BusNumber = "BUS001", OperatorName = "TestOperator", IsActive = false }
            };

            _mockBusService.Setup(x => x.GetByOperatorAsync("TestOperator", 1, 10))
                .ReturnsAsync((buses, 1));

            // Act
            var result = await _controller.GetByOperator(request);

            // Assert
            result.Should().NotBeNull();
            _mockBusService.Verify(x => x.GetByOperatorAsync("TestOperator", 1, 10), Times.Once);
        }
    }

    public class SourceControllerTests
    {
        private readonly Mock<SourceService> _mockSourceService;
        private readonly SourceController _controller;

        public SourceControllerTests()
        {
            _mockSourceService = new Mock<SourceService>(null);
            _controller = new SourceController(_mockSourceService.Object);
        }

        [Fact]
        public async Task GetAllSources_WithValidPagination_ShouldReturnSources()
        {
            // Arrange
            var sources = new List<SourceResponseDto>
            {
                new SourceResponseDto { SourceId = 1, SourceName = "Delhi", IsActive = true },
                new SourceResponseDto { SourceId = 2, SourceName = "Mumbai", IsActive = true }
            };

            var request = new PaginationRequest { PageNumber = 1, PageSize = 10 };

            _mockSourceService.Setup(x => x.GetAllSourcesAsync(1, 10))
                .ReturnsAsync(sources);

            // Act
            var result = await _controller.GetAllSources(request);

            // Assert
            result.Should().NotBeNull();
        }
    }

    public class DestinationControllerTests
    {
        private readonly Mock<DestinationService> _mockDestinationService;
        private readonly DestinationController _controller;

        public DestinationControllerTests()
        {
            _mockDestinationService = new Mock<DestinationService>(null);
            _controller = new DestinationController(_mockDestinationService.Object);
        }

        [Fact]
        public async Task GetAllDestinations_WithValidPagination_ShouldReturnDestinations()
        {
            // Arrange
            var destinations = new List<DestinationResponseDto>
            {
                new DestinationResponseDto { DestinationId = 1, DestinationName = "Bangalore", IsActive = true },
                new DestinationResponseDto { DestinationId = 2, DestinationName = "Chennai", IsActive = true }
            };

            var request = new PaginationRequest { PageNumber = 1, PageSize = 10 };

            _mockDestinationService.Setup(x => x.GetAllDestinationsAsync(1, 10))
                .ReturnsAsync(destinations);

            // Act
            var result = await _controller.GetAllDestinations(request);

            // Assert
            result.Should().NotBeNull();
        }
    }
}

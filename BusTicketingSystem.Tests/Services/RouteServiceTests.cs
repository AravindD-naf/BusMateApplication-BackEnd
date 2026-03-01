using BusTicketingSystem.DTOs.Requests;
using BusTicketingSystem.Exceptions;
using BusTicketingSystem.Interfaces.Repositories;
using BusTicketingSystem.Models;
using BusTicketingSystem.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace BusTicketingSystem.Tests.Services
{
    public class RouteServiceTests
    {
        private readonly Mock<IRouteRepository> _mockRouteRepository;
        private readonly Mock<IAuditRepository> _mockAuditRepository;
        private readonly RouteService _routeService;

        public RouteServiceTests()
        {
            _mockRouteRepository = new Mock<IRouteRepository>();
            _mockAuditRepository = new Mock<IAuditRepository>();

            _routeService = new RouteService(
                _mockRouteRepository.Object,
                _mockAuditRepository.Object
            );
        }

        #region CreateRouteAsync Tests

        [Fact]
        public async Task CreateRouteAsync_WithValidInput_ShouldCreateRoute()
        {
            // Arrange
            var request = new RouteCreateRequestDto
            {
                Source = "New York",
                Destination = "Boston",
                Distance = 215.5m,
                EstimatedTravelTimeMinutes = 240,
                BaseFare = 50m
            };

            _mockRouteRepository.Setup(r =>
                r.GetBySourceDestinationAsync(request.Source.Trim(), request.Destination.Trim()))
                .ReturnsAsync((Route)null);

            // Act
            var result = await _routeService.CreateRouteAsync(request, 1, "127.0.0.1");

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            _mockRouteRepository.Verify(r => r.AddAsync(It.IsAny<Route>()), Times.Once);
        }

        [Fact]
        public async Task CreateRouteAsync_WithDuplicateRoute_ShouldThrowConflictException()
        {
            // Arrange
            var request = new RouteCreateRequestDto
            {
                Source = "New York",
                Destination = "Boston",
                Distance = 215.5m,
                EstimatedTravelTimeMinutes = 240,
                BaseFare = 50m
            };

            var existingRoute = new Route
            {
                RouteId = 1,
                Source = "New York",
                Destination = "Boston",
                IsDeleted = false
            };

            _mockRouteRepository.Setup(r =>
                r.GetBySourceDestinationAsync(request.Source.Trim(), request.Destination.Trim()))
                .ReturnsAsync(existingRoute);

            // Act & Assert
            await Assert.ThrowsAsync<ConflictException>(() =>
                _routeService.CreateRouteAsync(request, 1, "127.0.0.1"));
        }

        [Fact]
        public async Task CreateRouteAsync_WithValidDistanceAndFare_ShouldIncludeInCreatedRoute()
        {
            // Arrange
            var request = new RouteCreateRequestDto
            {
                Source = "NYC",
                Destination = "LA",
                Distance = 2800.5m,
                EstimatedTravelTimeMinutes = 1200,
                BaseFare = 150m
            };

            _mockRouteRepository.Setup(r =>
                r.GetBySourceDestinationAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((Route)null);

            // Act
            var result = await _routeService.CreateRouteAsync(request, 1, "127.0.0.1");

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            _mockRouteRepository.Verify(r => r.AddAsync(It.Is<Route>(route =>
                route.Distance == 2800.5m &&
                route.EstimatedTravelTimeMinutes == 1200 &&
                route.BaseFare == 150m)), Times.Once);
        }

        #endregion

        #region UpdateRouteAsync Tests

        [Fact]
        public async Task UpdateRouteAsync_WithValidInput_ShouldUpdateRoute()
        {
            // Arrange
            var routeId = 1;
            var request = new RouteUpdateRequestDto
            {
                Source = "New York",
                Destination = "Washington",
                Distance = 225m,
                EstimatedTravelTimeMinutes = 300,
                BaseFare = 60m
            };

            var existingRoute = new Route
            {
                RouteId = routeId,
                Source = "New York",
                Destination = "Boston",
                Distance = 215.5m,
                EstimatedTravelTimeMinutes = 240,
                BaseFare = 50m
            };

            _mockRouteRepository.Setup(r => r.GetByIdAsync(routeId))
                .ReturnsAsync(existingRoute);

            // Act
            var result = await _routeService.UpdateRouteAsync(routeId, request, 1, "127.0.0.1");

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            _mockRouteRepository.Verify(r => r.UpdateAsync(It.IsAny<Route>()), Times.Once);
        }

        [Fact]
        public async Task UpdateRouteAsync_WithInvalidId_ShouldThrowResourceNotFoundException()
        {
            // Arrange
            var request = new RouteUpdateRequestDto
            {
                Source = "New York",
                Destination = "Boston",
                Distance = 215.5m,
                EstimatedTravelTimeMinutes = 240,
                BaseFare = 50m
            };

            _mockRouteRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Route)null);

            // Act & Assert
            await Assert.ThrowsAsync<ResourceNotFoundException>(() =>
                _routeService.UpdateRouteAsync(999, request, 1, "127.0.0.1"));
        }

        #endregion

        #region GetRouteAsync Tests

        [Fact]
        public async Task GetRouteAsync_WithValidId_ShouldReturnRoute()
        {
            // Arrange
            var routeId = 1;
            var route = new Route
            {
                RouteId = routeId,
                Source = "New York",
                Destination = "Boston",
                Distance = 215.5m,
                EstimatedTravelTimeMinutes = 240,
                BaseFare = 50m
            };

            _mockRouteRepository.Setup(r => r.GetByIdAsync(routeId))
                .ReturnsAsync(route);

            // Act
            var result = await _routeService.GetRouteAsync(routeId);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Distance.Should().Be(215.5m);
            result.Data.BaseFare.Should().Be(50m);
        }

        #endregion

        #region GetAllRoutesAsync Tests

        [Fact]
        public async Task GetAllRoutesAsync_ShouldReturnAllRoutes()
        {
            // Arrange
            var routes = new List<Route>
            {
                new Route { RouteId = 1, Source = "NYC", Destination = "Boston", Distance = 215.5m },
                new Route { RouteId = 2, Source = "NYC", Destination = "DC", Distance = 225m }
            };

            _mockRouteRepository.Setup(r => r.GetAllAsync())
                .ReturnsAsync(routes);

            // Act
            var result = await _routeService.GetAllRoutesAsync();

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().HaveCount(2);
        }

        #endregion
    }
}

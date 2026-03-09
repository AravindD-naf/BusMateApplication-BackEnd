using BusTicketingSystem.Services;
using Moq;
using Xunit;
using FluentAssertions;
using BusTicketingSystem.DTOs.Requests;
using BusTicketingSystem.Repositories;

namespace BusTicketingSystem.Tests.Integration
{
    public class BusBookingIntegrationTests
    {
        private readonly Mock<IBusRepository> _mockBusRepository;
        private readonly Mock<ISourceRepository> _mockSourceRepository;
        private readonly Mock<IDestinationRepository> _mockDestinationRepository;
        private readonly BusService _busService;
        private readonly SourceService _sourceService;
        private readonly DestinationService _destinationService;

        public BusBookingIntegrationTests()
        {
            _mockBusRepository = new Mock<IBusRepository>();
            _mockSourceRepository = new Mock<ISourceRepository>();
            _mockDestinationRepository = new Mock<IDestinationRepository>();
            
            var mockAuditService = new Mock<IAuditService>();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            
            _busService = new BusService(_mockBusRepository, mockAuditService, mockHttpContextAccessor);
            _sourceService = new SourceService(_mockSourceRepository);
            _destinationService = new DestinationService(_mockDestinationRepository);

            // Setup audit service
            mockAuditService.Setup(x => x.LogAsync(It.IsAny<int>(), It.IsAny<string>(), 
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), 
                It.IsAny<object>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);
        }

        [Fact]
        public async Task CompleteWorkflow_CreateBus_CreateSourceDestination_ShouldSucceed()
        {
            // Arrange
            var busRequest = new CreateBusRequest
            {
                BusNumber = "BUS001",
                BusType = "AC",
                TotalSeats = 40,
                OperatorName = "TestOperator"
            };

            var sourceRequest = new CreateSourceRequest
            {
                SourceName = "Delhi",
                Description = "Capital City"
            };

            var destinationRequest = new CreateDestinationRequest
            {
                DestinationName = "Mumbai",
                Description = "Financial Hub"
            };

            _mockBusRepository.Setup(x => x.GetByBusNumberAsync(It.IsAny<string>()))
                .ReturnsAsync((Models.Bus)null);
            
            _mockBusRepository.Setup(x => x.CreateAsync(It.IsAny<Models.Bus>()))
                .Returns(Task.CompletedTask);

            _mockSourceRepository.Setup(x => x.CreateAsync(It.IsAny<Models.Source>()))
                .Returns(Task.CompletedTask);

            _mockDestinationRepository.Setup(x => x.CreateAsync(It.IsAny<Models.Destination>()))
                .Returns(Task.CompletedTask);

            // Act
            var busResult = await _busService.CreateBusAsync(busRequest, 1, "192.168.1.1");
            var sourceResult = await _sourceService.CreateSourceAsync(sourceRequest);
            var destinationResult = await _destinationService.CreateDestinationAsync(destinationRequest);

            // Assert
            busResult.Should().NotBeNull();
            busResult.BusNumber.Should().Be("BUS001");
            busResult.IsActive.Should().BeFalse(); // Should be inactive initially
            busResult.TotalSeats.Should().Be(40);

            sourceResult.Should().NotBeNull();
            sourceResult.SourceName.Should().Be("Delhi");
            sourceResult.IsActive.Should().BeTrue();

            destinationResult.Should().NotBeNull();
            destinationResult.DestinationName.Should().Be("Mumbai");
            destinationResult.IsActive.Should().BeTrue();
        }

        [Fact]
        public async Task MultipleOperators_ShouldCreateBusesForEachOperator()
        {
            // Arrange
            var operators = new[] { "Operator1", "Operator2", "Operator3" };
            var buses = new List<Models.Bus>();

            _mockBusRepository.Setup(x => x.GetByBusNumberAsync(It.IsAny<string>()))
                .ReturnsAsync((Models.Bus)null);

            _mockBusRepository.Setup(x => x.CreateAsync(It.IsAny<Models.Bus>()))
                .Callback<Models.Bus>(b => buses.Add(b))
                .Returns(Task.CompletedTask);

            // Act
            int counter = 1;
            foreach (var op in operators)
            {
                var request = new CreateBusRequest
                {
                    BusNumber = $"BUS{counter:D3}",
                    BusType = "AC",
                    TotalSeats = 40,
                    OperatorName = op
                };
                await _busService.CreateBusAsync(request, 1, "192.168.1.1");
                counter++;
            }

            // Assert
            buses.Should().HaveCount(3);
            buses.Select(b => b.OperatorName).Should().BeEquivalentTo(operators);
            buses.Should().AllSatisfy(b => b.IsActive.Should().BeFalse());
            buses.Should().AllSatisfy(b => b.TotalSeats.Should().Be(40));
        }
    }
}

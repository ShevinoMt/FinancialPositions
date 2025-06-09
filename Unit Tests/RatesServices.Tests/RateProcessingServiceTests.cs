using FinancialPositions.Core.Events;
using FinancialPositions.RatesServices.Core.Entities;
using FinancialPositions.RatesServices.Core.Repositories;
using FinancialPositions.RatesServices.Services.CoinMarketCap;
using FinancialPositions.RatesServices.Services.Core;
using FinancialPositions.Shared.Core.Infrastructure;
using Microsoft.Extensions.Logging;
using Moq;

namespace RatesServices.Tests
{
    public class RateProcessingServiceTests
    {
        private readonly Mock<IRatesService> _ratesServiceMock;
        private readonly Mock<IRateRepository> _rateRepositoryMock;
        private readonly Mock<IMessageBus> _messageBusMock;
        private readonly Mock<ILogger<RateProcessingService>> _loggerMock;
        private readonly RateProcessingService _service;

        public RateProcessingServiceTests()
        {
            _ratesServiceMock = new Mock<IRatesService>();
            _rateRepositoryMock = new Mock<IRateRepository>();
            _messageBusMock = new Mock<IMessageBus>();
            _loggerMock = new Mock<ILogger<RateProcessingService>>();

            _service = new RateProcessingService(
                _ratesServiceMock.Object,
                _rateRepositoryMock.Object,
                _messageBusMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task ProcessRatesAsync_WhenRateChangeExceedsThreshold_PublishesEvent()
        {
            // Arrange
            var symbol = "BTC";
            var oldRate = 50000m;
            var newRate = 55000m; // 10% increase

            var latestRates = new List<(string symbol, decimal rate)>
            {
                (symbol, newRate)
            };

            _ratesServiceMock
                .Setup(x => x.GetLatestRatesAsync())
                .ReturnsAsync(latestRates);

            _rateRepositoryMock
                .Setup(x => x.GetLatestRateWithin24HoursAsync(symbol, It.IsAny<DateTime>()))
                .ReturnsAsync(new Rate { Symbol = symbol, Value = oldRate });

            // Act
            await _service.ProcessRatesAsync();

            // Assert
            _messageBusMock.Verify(x => x.PublishAsync(
                It.Is<RateChangedEvent>(e =>
                    e.InstrumentId == $"{symbol}/USD" &&
                    e.OldRate == oldRate &&
                    e.NewRate == newRate),
                "rate-changes"),
                Times.Once);
        }

        [Fact]
        public async Task ProcessRatesAsync_WhenRateChangeDoesNotExceedThreshold_DoesNotPublishEvent()
        {
            // Arrange
            var symbol = "BTC";
            var oldRate = 50000m;
            var newRate = 51000m; // 2% increase

            var latestRates = new List<(string symbol, decimal rate)>
            {
                (symbol, newRate)
            };

            _ratesServiceMock
                .Setup(x => x.GetLatestRatesAsync())
                .ReturnsAsync(latestRates);

            _rateRepositoryMock
                .Setup(x => x.GetLatestRateWithin24HoursAsync(symbol))
                .ReturnsAsync(new Rate { Symbol = symbol, Value = oldRate });

            _rateRepositoryMock
                .Setup(x => x.GetLatestRateWithin24HoursAsync(symbol, It.IsAny<DateTime>()))
                .ReturnsAsync(new Rate { Symbol = symbol, Value = oldRate });

            // Act
            await _service.ProcessRatesAsync();

            // Assert
            _messageBusMock.Verify(x => x.PublishAsync(
                It.IsAny<RateChangedEvent>(),
                It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task ProcessRatesAsync_WhenNoOldRateExists_DoesNotPublishEvent()
        {
            // Arrange
            var symbol = "BTC";
            var newRate = 55000m;

            var latestRates = new List<(string symbol, decimal rate)>
            {
                (symbol, newRate)
            };

            _ratesServiceMock
                .Setup(x => x.GetLatestRatesAsync())
                .ReturnsAsync(latestRates);

            _rateRepositoryMock
                .Setup(x => x.GetLatestRateWithin24HoursAsync(symbol))
                .ReturnsAsync((Rate?)null);

            // Act
            await _service.ProcessRatesAsync();

            // Assert
            _messageBusMock.Verify(x => x.PublishAsync(
                It.IsAny<RateChangedEvent>(),
                It.IsAny<string>()),
                Times.Never);
        }
    }
}
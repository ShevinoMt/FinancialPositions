using FinancialPositions.Core.Events;
using FinancialPositions.PositionsServices.Core.Entities;
using FinancialPositions.PositionsServices.Core.Enums;
using FinancialPositions.PositionsServices.Core.Repositories;
using FinancialPositions.PositionsServices.Services;
using FinancialPositions.Shared.Core.Infrastructure;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatesServices.Tests
{
    public class PositionServiceTests
    {
        private readonly Mock<IPositionRepository> _positionRepositoryMock;
        private readonly Mock<IMessageBus> _messageBusMock;
        private readonly Mock<ILogger<PositionService>> _loggerMock;
        private readonly PositionService _service;

        public PositionServiceTests()
        {
            _positionRepositoryMock = new Mock<IPositionRepository>();
            _messageBusMock = new Mock<IMessageBus>();
            _loggerMock = new Mock<ILogger<PositionService>>();

            _service = new PositionService(
                _positionRepositoryMock.Object,
                _messageBusMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task RecalculatePositionsAsync_CalculatesCorrectProfitLossForBuyPosition()
        {
            // Arrange
            var instrumentId = "BTC/USD";
            var position = new Position
            {
                Id = Guid.NewGuid(),
                InstrumentId = instrumentId,
                Quantity = 1m,
                InitialRate = 50000m,
                Side = PositionSide.Buy,
                IsClosed = false
            };

            var rateEvent = new RateChangedEvent
            {
                InstrumentId = instrumentId,
                NewRate = 55000m
            };

            _positionRepositoryMock
                .Setup(x => x.GetActivePositionsByInstrumentAsync(instrumentId))
                .ReturnsAsync(new[] { position });

            // Act
            await _service.RecalculatePositionsAsync(rateEvent);

            // Assert
            _messageBusMock.Verify(x => x.PublishAsync(
                It.Is<PositionValueCalculatedEvent>(e =>
                    e.PositionId == position.Id &&
                    e.ProfitLoss == 5000m), // (55000 - 50000) * 1 * 1
                "position-values"),
                Times.Once);
        }

        [Fact]
        public async Task RecalculatePositionsAsync_CalculatesCorrectProfitLossForSellPosition()
        {
            // Arrange
            var instrumentId = "BTC/USD";
            var position = new Position
            {
                Id = Guid.NewGuid(),
                InstrumentId = instrumentId,
                Quantity = 1m,
                InitialRate = 50000m,
                Side = PositionSide.Sell,
                IsClosed = false
            };

            var rateEvent = new RateChangedEvent
            {
                InstrumentId = instrumentId,
                NewRate = 55000m
            };

            _positionRepositoryMock
                .Setup(x => x.GetActivePositionsByInstrumentAsync(instrumentId))
                .ReturnsAsync(new[] { position });

            // Act
            await _service.RecalculatePositionsAsync(rateEvent);

            // Assert
            _messageBusMock.Verify(x => x.PublishAsync(
                It.Is<PositionValueCalculatedEvent>(e =>
                    e.PositionId == position.Id &&
                    e.ProfitLoss == -5000m), // (55000 - 50000) * 1 * -1
                "position-values"),
                Times.Once);
        }

        [Fact]
        public async Task AddPositionAsync_CreatesNewPosition()
        {
            // Arrange
            var positionEvent = new PositionAddedEvent
            {
                PositionId = Guid.NewGuid(),
                InstrumentId = "BTC/USD",
                Quantity = 1m,
                InitialRate = 50000m,
                Side = PositionSide.Buy,
                Timestamp = DateTime.UtcNow
            };

            // Act
            var result = await _service.AddPositionAsync(positionEvent);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(positionEvent.PositionId, result.Id);
            Assert.Equal(positionEvent.InstrumentId, result.InstrumentId);
            Assert.Equal(positionEvent.Quantity, result.Quantity);
            Assert.Equal(positionEvent.InitialRate, result.InitialRate);
            Assert.Equal(PositionSide.Buy, result.Side);
            Assert.False(result.IsClosed);

            _positionRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Position>()), Times.Once);
        }

        [Fact]
        public async Task ClosePositionAsync_ClosesExistingPosition()
        {
            // Arrange
            var positionId = Guid.NewGuid();
            var position = new Position
            {
                Id = positionId,
                IsClosed = false
            };

            var closedEvent = new PositionClosedEvent
            {
                PositionId = positionId,
                Timestamp = DateTime.UtcNow
            };

            _positionRepositoryMock
                .Setup(x => x.GetByIdAsync(positionId))
                .ReturnsAsync(position);

            // Act
            await _service.ClosePositionAsync(closedEvent);

            // Assert
            Assert.True(position.IsClosed);
            Assert.NotNull(position.ClosedAt);
            _positionRepositoryMock.Verify(x => x.UpdateAsync(position), Times.Once);
        }
    }
}

using FinancialPositions.Core.Events;
using FinancialPositions.PositionsServices.Core.Entities;
using FinancialPositions.PositionsServices.Core.Enums;
using FinancialPositions.PositionsServices.Core.Repositories;
using FinancialPositions.PositionsServices.Services.Core;
using FinancialPositions.Shared.Core.Infrastructure;
using Microsoft.Extensions.Logging;

namespace FinancialPositions.PositionsServices.Services
{
    public class PositionService : IPositionService
    {
        private readonly IPositionRepository positionRepository;
        private readonly IMessageBus messageBus;
        private readonly ILogger<PositionService> logger;

        public PositionService(
            IPositionRepository positionRepository,
            IMessageBus messageBus,
            ILogger<PositionService> logger)
        {
            this.positionRepository = positionRepository;
            this.messageBus = messageBus;
            this.logger = logger;
        }

        /// <summary>
        /// Add a new position based on the PositionAddedEvent.
        /// </summary>
        /// <param name="positionEvent"></param>
        /// <returns></returns>
        public async Task<Position> AddPositionAsync(PositionAddedEvent positionEvent)
        {
            var position = new Position
            {
                Id = positionEvent.PositionId,
                InstrumentId = positionEvent.InstrumentId,
                Quantity = positionEvent.Quantity,
                InitialRate = positionEvent.InitialRate,
                Side = positionEvent.Side,
                CreatedAt = positionEvent.Timestamp,
                IsClosed = false
            };

            await positionRepository.AddAsync(position);
            logger.LogInformation($"Position {position.Id} added for {position.InstrumentId}");

            return position;
        }

        /// <summary>
        /// Close an existing position based on the PositionClosedEvent.
        /// </summary>
        /// <param name="closedEvent"></param>
        /// <returns></returns>
        public async Task ClosePositionAsync(PositionClosedEvent closedEvent)
        {
            var position = await positionRepository.GetByIdAsync(closedEvent.PositionId);

            if (position == null)
            {
                logger.LogWarning($"Position {closedEvent.PositionId} not found");
                return;
            }

            position.IsClosed = true;
            position.ClosedAt = closedEvent.Timestamp;

            await positionRepository.UpdateAsync(position);
            logger.LogInformation($"Position {position.Id} closed");
        }

        /// <summary>
        /// Recalculate positions based on the RateChangedEvent.
        /// </summary>
        /// <param name="rateEvent"></param>
        /// <returns></returns>
        public async Task RecalculatePositionsAsync(RateChangedEvent rateEvent)
        {
            logger.LogInformation($"Recalculating positions for {rateEvent.InstrumentId} at rate {rateEvent.NewRate}");

            var positions = await positionRepository
                .GetActivePositionsByInstrumentAsync(rateEvent.InstrumentId);

            foreach (var position in positions)
            {
                var profitLoss = position.CalculateProfitLoss(rateEvent.NewRate);

                logger.LogInformation(
                    $"Position {position.Id} ({position.Side} {position.Quantity} {position.InstrumentId}) " +
                    $"P/L: {profitLoss:C2} at rate {rateEvent.NewRate}");

                var valueCalculatedEvent = new PositionValueCalculatedEvent
                {
                    PositionId = position.Id,
                    InstrumentId = position.InstrumentId,
                    ProfitLoss = profitLoss,
                    CurrentRate = rateEvent.NewRate,
                    Timestamp = DateTime.UtcNow
                };

                await messageBus.PublishAsync(valueCalculatedEvent, "position-values");
            }
        }
    }
}

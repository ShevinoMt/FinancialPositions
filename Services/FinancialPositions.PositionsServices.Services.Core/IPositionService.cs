using FinancialPositions.Core.Events;
using FinancialPositions.PositionsServices.Core.Entities;

namespace FinancialPositions.PositionsServices.Services.Core
{
    public interface IPositionService
    {
        /// <summary>
        /// Add a new position based on the PositionAddedEvent.
        /// </summary>
        /// <param name="positionEvent"></param>
        /// <returns></returns>
        Task<Position> AddPositionAsync(PositionAddedEvent positionEvent);

        /// <summary>
        /// Close an existing position based on the PositionClosedEvent.
        /// </summary>
        /// <param name="closedEvent"></param>
        /// <returns></returns>
        Task ClosePositionAsync(PositionClosedEvent closedEvent);

        /// <summary>
        /// Recalculate positions based on the latest rate changes.
        /// </summary>
        /// <param name="rateEvent"></param>
        /// <returns></returns>
        Task RecalculatePositionsAsync(RateChangedEvent rateEvent);
    }
}

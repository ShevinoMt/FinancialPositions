using FinancialPositions.PositionsServices.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialPositions.PositionsServices.Core.Repositories
{
    public interface IPositionRepository
    {
        /// <summary>
        /// Get position by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Position?> GetByIdAsync(Guid id);

        /// <summary>
        /// Get active positions for a specific instrument
        /// </summary>
        /// <param name="instrumentId"></param>
        /// <returns></returns>
        Task<IEnumerable<Position>> GetActivePositionsByInstrumentAsync(string instrumentId);

        /// <summary>
        /// Add a new position
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        Task<Position> AddAsync(Position position);

        /// <summary>
        /// Update an existing position
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        Task UpdateAsync(Position position);

        /// <summary>
        /// Get all active positions
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Position>> GetAllActivePositionsAsync();
    }
}

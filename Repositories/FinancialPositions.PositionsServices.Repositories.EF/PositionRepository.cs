using FinancialPositions.PositionsServices.Core.Entities;
using FinancialPositions.PositionsServices.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialPositions.PositionsServices.Repositories.EF
{
    public class PositionRepository : IPositionRepository
    {
        private readonly PositionsDbContext context;

        public PositionRepository(PositionsDbContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Get position by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Position?> GetByIdAsync(Guid id)
        {
            return await context.Positions.FindAsync(id);
        }

        /// <summary>
        /// Get active positions for a specific instrument
        /// </summary>
        /// <param name="instrumentId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Position>> GetActivePositionsByInstrumentAsync(string instrumentId)
        {
            return await context.Positions
                .Where(p => p.InstrumentId == instrumentId && !p.IsClosed)
                .ToListAsync();
        }

        /// <summary>
        /// Add a new position
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public async Task<Position> AddAsync(Position position)
        {
            context.Positions.Add(position);
            await context.SaveChangesAsync();
            return position;
        }

        /// <summary>
        /// Update an existing position
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public async Task UpdateAsync(Position position)
        {
            context.Positions.Update(position);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Get all active positions
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Position>> GetAllActivePositionsAsync()
        {
            return await context.Positions
                .Where(p => !p.IsClosed)
                .ToListAsync();
        }
    }
}

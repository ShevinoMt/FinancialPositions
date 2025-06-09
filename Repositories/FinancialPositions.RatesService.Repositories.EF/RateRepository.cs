using FinancialPositions.RatesServices.Core.Entities;
using FinancialPositions.RatesServices.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialPositions.RatesService.Repositories.EF
{
    /// <summary>
    /// Repository for managing rates using Entity Framework Core.
    /// </summary>
    public class RateRepository : IRateRepository
    {
        private readonly RatesDbContext context;
        public RateRepository(RatesDbContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Add a new rate to the repository.
        /// </summary>
        /// <param name="rate"></param>
        /// <returns></returns>
        public async Task<Rate> AddAsync(Rate rate)
        {
            context.Rates.Add(rate);
            await context.SaveChangesAsync();
            return rate;
        }

        /// <summary>
        /// Get rates by symbol since a specific date.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="since"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Rate>> GetBySymbolAsync(string symbol, DateTime since)
        {
            return await context.Rates
            .Where(r => r.Symbol == symbol && r.Timestamp >= since)
            .OrderBy(r => r.Timestamp)
            .ToListAsync();
        }

        /// <summary>
        /// Get the latest rate for a symbol within the last 24 hours, before a specific cutoff time.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="cutoff"></param>
        /// <returns></returns>
        public async Task<Rate?> GetLatestRateWithin24HoursAsync(string symbol, DateTime cutoff)
        {
            var since = DateTime.UtcNow.AddHours(-24);
            return await context.Rates
            .Where(r => r.Symbol == symbol && r.Timestamp >= since && r.Timestamp < cutoff)
            .OrderBy(r => r.Timestamp)
            .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Get the latest rate for a symbol within the last 24 hours.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public async Task<Rate?> GetLatestRateWithin24HoursAsync(string symbol)
        {
            var since = DateTime.UtcNow.AddHours(-24);
            return await context.Rates
            .Where(r => r.Symbol == symbol && r.Timestamp >= since)
            .OrderBy(r => r.Timestamp)
            .FirstOrDefaultAsync();
        }
    }
}

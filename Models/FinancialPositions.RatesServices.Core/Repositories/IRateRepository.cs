using FinancialPositions.RatesServices.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialPositions.RatesServices.Core.Repositories
{
    public interface IRateRepository
    {
        /// <summary>
        /// Add a new rate
        /// </summary>
        /// <param name="rate"></param>
        /// <returns></returns>
        Task<Rate> AddAsync(Rate rate);

        /// <summary>
        /// Get rate by ID
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="since"></param>
        /// <returns></returns>
        Task<IEnumerable<Rate>> GetBySymbolAsync(string symbol, DateTime since);

        /// <summary>
        /// Get the latest rate for a symbol within the last 24 hours, before a specific cutoff time.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="cutoff"></param>
        /// <returns></returns>
        Task<Rate?> GetLatestRateWithin24HoursAsync(string symbol, DateTime cutoff);

        /// <summary>
        /// Get the latest rate for a symbol within the last 24 hours.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        Task<Rate?> GetLatestRateWithin24HoursAsync(string symbol);
    }
}

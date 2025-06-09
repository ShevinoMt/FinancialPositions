using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialPositions.RatesServices.Services.Core
{
    public interface IRateProcessingService
    {
        /// <summary>
        /// Fetch the latest rates, check for significant changes, and publish events if necessary.
        /// </summary>
        /// <returns></returns>
        Task ProcessRatesAsync();
    }
}

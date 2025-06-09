using FinancialPositions.Core.Events;
using FinancialPositions.RatesServices.Core.Repositories;
using FinancialPositions.RatesServices.Services.Core;
using FinancialPositions.Shared.Core.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialPositions.RatesServices.Services.CoinMarketCap
{
    /// <summary>
    /// Service for processing cryptocurrency rates, checking for significant changes, and publishing events.
    /// </summary>
    public class RateProcessingService : IRateProcessingService
    {
        private readonly IRatesService ratesService;
        private readonly IRateRepository rateRepository;
        private readonly IMessageBus messageBus;
        private readonly ILogger<RateProcessingService> logger;
        private const decimal VariationThreshold = 0.05m; // 5%

        public RateProcessingService(
            IRatesService ratesService,
            IRateRepository rateRepository,
            IMessageBus messageBus,
            ILogger<RateProcessingService> logger)
        {
            this.ratesService = ratesService;
            this.rateRepository = rateRepository;
            this.messageBus = messageBus;
            this.logger = logger;
        }

        /// <summary>
        /// Fetch the latest rates, check for significant changes, and publish events if necessary.
        /// </summary>
        /// <returns></returns>
        public async Task ProcessRatesAsync()
        {
            try
            {
                logger.LogInformation("Starting rate processing");
                var latestRates = await ratesService.GetLatestRatesAsync();

                foreach (var (symbol, currentRate) in latestRates)
                {
                    // Save the new rate
                    var newRate = new RatesServices.Core.Entities.Rate
                    {
                        Id = Guid.NewGuid(),
                        Symbol = symbol,
                        QuoteCurrency = "USD",
                        Value = currentRate,    
                        Timestamp = DateTime.UtcNow
                    };
                    await rateRepository.AddAsync(newRate);
                    logger.LogInformation($"Saved rate for {symbol}: {currentRate}");

                    // Check for significant variation
                    var oldestRate = await rateRepository.GetLatestRateWithin24HoursAsync(symbol, cutoff: newRate.Timestamp);

                    if (oldestRate != null)
                    {
                        var percentageChange = CalculatePercentageChange(oldestRate.Value, currentRate);

                        if (Math.Abs(percentageChange) > VariationThreshold)
                        {
                            logger.LogInformation(
                                $"Significant rate change detected for {symbol}: {percentageChange:P2} " +
                                $"(Old: {oldestRate.Value}, New: {currentRate})");

                            var rateChangedEvent = new RateChangedEvent
                            {
                                InstrumentId = newRate.InstrumentId,
                                OldRate = oldestRate.Value,
                                NewRate = currentRate,
                                PercentageChange = percentageChange,
                                Timestamp = DateTime.UtcNow
                            };

                            await messageBus.PublishAsync(rateChangedEvent, "rate-changes");
                        }
                    }
                }
                logger.LogInformation("Rate processing completed");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing rates");
                throw;
            }
        }

        /// <summary>
        /// Calculate the percentage change between two rates.
        /// </summary>
        /// <param name="oldRate"></param>
        /// <param name="newRate"></param>
        /// <returns></returns>
        private decimal CalculatePercentageChange(decimal oldRate, decimal newRate)
        {
            if (oldRate == 0) return 0;
            return (newRate - oldRate) / oldRate;
        }
    }
}

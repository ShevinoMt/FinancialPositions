namespace FinancialPositions.RatesServices.Services.Core
{
    public interface IRatesService
    {
        /// <summary>
        /// Fetch the latest cryptocurrency rates from an external service.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<(string symbol, decimal rate)>> GetLatestRatesAsync();
    }
}

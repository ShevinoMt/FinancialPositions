using FinancialPositions.RatesServices.Services.CoinMarketCap.Models;
using FinancialPositions.RatesServices.Services.Core;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FinancialPositions.RatesServices.Services.CoinMarketCap
{
    /// <summary>
    /// Service to fetch cryptocurrency rates from CoinMarketCap API.
    /// </summary>
    public class CoinMarketCapService : IRatesService
    {
        private readonly HttpClient httpClient;
        private readonly string apiKey;
        private const string BaseUrl = "https://pro-api.coinmarketcap.com/v1/";


        public CoinMarketCapService(HttpClient httpClient, IConfiguration configuration)
        {
            this.httpClient = httpClient;
            apiKey = configuration["CoinMarketCap:ApiKey"] ?? throw new InvalidOperationException("CoinMarketCap API key not configured");
            this.httpClient.DefaultRequestHeaders.Add("X-CMC_PRO_API_KEY", apiKey);
        }

        /// <summary>
        /// Fetch the latest cryptocurrency rates from CoinMarketCap API.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<(string symbol, decimal rate)>> GetLatestRatesAsync()
        {
            var response = await httpClient.GetAsync($"{BaseUrl}cryptocurrency/listings/latest?convert=USD&limit=10");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var data = JsonSerializer.Deserialize<CoinMarketCapResponse>(content, options);

            if (data?.Data == null)
                return Enumerable.Empty<(string, decimal)>();

            return data.Data
                .Where(crypto => crypto.Quote.ContainsKey("USD"))
                .Select(crypto => (
                    symbol: crypto.Symbol,
                    rate: crypto.Quote["USD"].Price
                ));
        }
    }

}

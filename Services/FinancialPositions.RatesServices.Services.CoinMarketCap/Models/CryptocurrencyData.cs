using System.Text.Json.Serialization;

namespace FinancialPositions.RatesServices.Services.CoinMarketCap.Models
{
    public class CryptocurrencyData
    {
        [JsonPropertyName("symbol")]
        public string Symbol { get; set; } = string.Empty;
        [JsonPropertyName("quote")]
        public Dictionary<string, QuoteData> Quote { get; set; } = new();
    }
}

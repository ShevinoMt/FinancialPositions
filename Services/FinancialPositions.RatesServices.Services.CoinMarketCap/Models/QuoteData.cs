using System.Text.Json.Serialization;

namespace FinancialPositions.RatesServices.Services.CoinMarketCap.Models
{
    public class QuoteData
    {
        [JsonPropertyName("price")]
        public decimal Price { get; set; }
    }
}

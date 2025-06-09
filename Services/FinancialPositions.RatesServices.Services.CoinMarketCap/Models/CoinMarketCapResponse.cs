using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FinancialPositions.RatesServices.Services.CoinMarketCap.Models
{
    public class CoinMarketCapResponse
    {
        [JsonPropertyName("data")]
        public List<CryptocurrencyData> Data { get; set; } = new();
    }
}

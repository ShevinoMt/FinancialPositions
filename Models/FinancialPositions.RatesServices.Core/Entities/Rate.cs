namespace FinancialPositions.RatesServices.Core.Entities
{
    public class Rate
    {
        public Guid Id { get; set; }
        public string Symbol { get; set; } = string.Empty;
        public string QuoteCurrency { get; set; } = string.Empty;
        public string InstrumentId => $"{Symbol}/{QuoteCurrency}";
        public decimal Value { get; set; }
        public DateTime Timestamp { get; set; }
    }
}

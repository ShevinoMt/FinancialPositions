namespace FinancialPositions.Core.Events
{
    public class RateChangedEvent
    {
        public string InstrumentId { get; set; } = string.Empty;
        public decimal OldRate { get; set; }
        public decimal NewRate { get; set; }
        public decimal PercentageChange { get; set; }
        public DateTime Timestamp { get; set; }
    }
}

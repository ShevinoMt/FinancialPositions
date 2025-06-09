namespace FinancialPositions.Core.Events
{
    public class PositionValueCalculatedEvent
    {
        public Guid PositionId { get; set; }
        public string InstrumentId { get; set; } = string.Empty;
        public decimal ProfitLoss { get; set; }
        public decimal CurrentRate { get; set; }
        public DateTime Timestamp { get; set; }
    }
}

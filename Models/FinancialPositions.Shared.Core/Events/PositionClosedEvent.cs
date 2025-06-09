namespace FinancialPositions.Core.Events
{
    public class PositionClosedEvent
    {
        public Guid PositionId { get; set; }
        public DateTime Timestamp { get; set; }
    }
}

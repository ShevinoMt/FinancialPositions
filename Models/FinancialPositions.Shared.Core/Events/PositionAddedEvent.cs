using FinancialPositions.PositionsServices.Core.Enums;

namespace FinancialPositions.Core.Events
{
    public class PositionAddedEvent
    {
        public Guid PositionId { get; set; }
        public string InstrumentId { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal InitialRate { get; set; }
        public PositionSide Side { get; set; } 
        public DateTime Timestamp { get; set; }
    }
}

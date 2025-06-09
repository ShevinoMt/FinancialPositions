using FinancialPositions.PositionsServices.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialPositions.PositionsServices.Core.Entities
{
    public class Position
    {
        public Guid Id { get; set; }
        public string InstrumentId { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal InitialRate { get; set; }
        public PositionSide Side { get; set; }
        public bool IsClosed { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ClosedAt { get; set; }

        public decimal CalculateProfitLoss(decimal currentRate)
        {
            var sideMultiplier = Side == PositionSide.Buy ? 1 : -1;
            return Quantity * (currentRate - InitialRate) * sideMultiplier;
        }
    }
}

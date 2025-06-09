using FinancialPositions.PositionsServices.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace FinancialPositions.PositionsServices.API.Models
{
    /// <summary>
    /// Request model for adding a new position.
    /// </summary>
    public class AddPositionRequest
    {
        /// <summary>
        /// Instrument identifier for the position.  example "BTC-USD".
        /// </summary>
        public string InstrumentId { get; set; } = string.Empty;

        /// <summary>
        /// Quantity of the instrument to be added to the position.
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// Initial rate at which the position is opened. This is the price at which the instrument was bought or sold.
        /// </summary>
        public decimal InitialRate { get; set; }

        /// <summary>
        /// Side of the position, indicating whether it is a buy or sell position.
        /// </summary>
        [Required]
        public PositionSide Side { get; set; }
    }
}

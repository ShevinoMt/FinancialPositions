using FinancialPositions.PositionsServices.Core.Entities;
using FinancialPositions.PositionsServices.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatesServices.Tests
{
    public class PositionCalculationTests
    {
        [Theory]
        [InlineData(1, 50000, 55000, PositionSide.Buy, 5000)]
        [InlineData(1, 50000, 45000, PositionSide.Buy, -5000)]
        [InlineData(1, 50000, 55000, PositionSide.Sell, -5000)]
        [InlineData(1, 50000, 45000, PositionSide.Sell, 5000)]
        [InlineData(0.5, 50000, 60000, PositionSide.Buy, 5000)]
        [InlineData(2, 50000, 52500, PositionSide.Buy, 5000)]
        public void CalculateProfitLoss_ReturnsCorrectValue(
            decimal quantity,
            decimal initialRate,
            decimal currentRate,
            PositionSide side,
            decimal expectedProfitLoss)
        {
            // Arrange
            var position = new Position
            {
                Quantity = quantity,
                InitialRate = initialRate,
                Side = side
            };

            // Act
            var profitLoss = position.CalculateProfitLoss(currentRate);

            // Assert
            Assert.Equal(expectedProfitLoss, profitLoss);
        }
    }
}

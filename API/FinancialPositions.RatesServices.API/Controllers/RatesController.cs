using FinancialPositions.RatesServices.Services.Core;
using Microsoft.AspNetCore.Mvc;

namespace FinancialPositions.RatesServices.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RatesController : ControllerBase
    {
        private readonly IRateProcessingService _rateProcessingService;
        private readonly ILogger<RatesController> _logger;

        public RatesController(
            IRateProcessingService rateProcessingService,
            ILogger<RatesController> logger)
        {
            _rateProcessingService = rateProcessingService;
            _logger = logger;
        }

        /// <summary>
        /// Trigger rate fetching (simulates scheduler)
        /// </summary>
        /// <returns></returns>
        [HttpPost("fetch")]
        public async Task<IActionResult> FetchRates()
        {
            try
            {
                _logger.LogInformation("Rate fetch triggered");
                await _rateProcessingService.ProcessRatesAsync();
                return Ok(new { message = "Rates fetched and processed successfully", timestamp = DateTime.UtcNow });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching rates");
                return StatusCode(500, new { error = "An error occurred while fetching rates" });
            }
        }
    }
}

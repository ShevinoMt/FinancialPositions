using FinancialPositions.Core.Events;
using FinancialPositions.PositionsServices.API.Models;
using FinancialPositions.PositionsServices.Core.Repositories;
using FinancialPositions.Shared.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace FinancialPositions.PositionsServices.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class PositionsController : ControllerBase
    {
        private readonly IPositionRepository _positionRepository;
        private readonly IMessageBus _messageBus;
        private readonly ILogger<PositionsController> _logger;

        public PositionsController(
            IPositionRepository positionRepository,
            IMessageBus messageBus,
            ILogger<PositionsController> logger)
        {
            _positionRepository = positionRepository;
            _messageBus = messageBus;
            _logger = logger;
        }

        /// <summary>
        /// Get all active positions
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetActivePositions()
        {
            var positions = await _positionRepository.GetAllActivePositionsAsync();
            return Ok(positions);
        }

        /// <summary>
        /// Get specific position
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPosition(Guid id)
        {
            var position = await _positionRepository.GetByIdAsync(id);
            if (position == null)
                return NotFound();

            return Ok(position);
        }

        /// <summary>
        ///  Add a new position
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddPosition([FromBody] AddPositionRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var positionEvent = new PositionAddedEvent
            {
                PositionId = Guid.NewGuid(),
                InstrumentId = request.InstrumentId,
                Quantity = request.Quantity,
                InitialRate = request.InitialRate,
                Side = request.Side,
                Timestamp = DateTime.UtcNow
            };

            _logger.LogInformation($"Publishing position added event for {request.InstrumentId}");
            await _messageBus.PublishAsync(positionEvent, "position-added");

            return Ok(new { positionId = positionEvent.PositionId });
        }

        /// <summary>
        /// Close a position
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> ClosePosition(Guid id)
        {
            var position = await _positionRepository.GetByIdAsync(id);
            if (position == null)
                return NotFound();

            if (position.IsClosed)
                return BadRequest(new { error = "Position is already closed" });

            var closedEvent = new PositionClosedEvent
            {
                PositionId = id,
                Timestamp = DateTime.UtcNow
            };

            _logger.LogInformation($"Publishing position closed event for {id}");
            await _messageBus.PublishAsync(closedEvent, "position-closed");

            return Ok();
        }
    }

}

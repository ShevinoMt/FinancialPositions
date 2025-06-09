using FinancialPositions.Core.Events;
using FinancialPositions.PositionsServices.Services.Core;
using FinancialPositions.Shared.Core.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialPositions.PositionsServices.Services
{
    /// <summary>
    /// Handles incoming events on the message bus related to financial positions and processes them accordingly.
    /// </summary>
    public class PositionsEventHandlerService : IHostedService
    {
        private readonly IMessageBus _messageBus;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PositionsEventHandlerService> _logger;

        public PositionsEventHandlerService(
            IMessageBus messageBus,
            IServiceProvider serviceProvider,
            ILogger<PositionsEventHandlerService> logger)
        {
            _messageBus = messageBus;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        /// <summary>
        /// Starts the event handler service, subscribing to relevant events on the message bus.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting event handler service");

            await _messageBus.SubscribeAsync<RateChangedEvent>("rate-changes", HandleRateChange);
            await _messageBus.SubscribeAsync<PositionAddedEvent>("position-added", HandlePositionAdded);
            await _messageBus.SubscribeAsync<PositionClosedEvent>("position-closed", HandlePositionClosed);

            _logger.LogInformation("Event handlers registered");
        }

        /// <summary>
        /// Handles rate change events by recalculating positions based on the new rate.
        /// </summary>
        /// <param name="rateEvent"></param>
        /// <returns></returns>
        private async Task HandleRateChange(RateChangedEvent rateEvent)
        {
            _logger.LogInformation($"Received rate change event for {rateEvent.InstrumentId}");

            using var scope = _serviceProvider.CreateScope();
            var positionService = scope.ServiceProvider.GetRequiredService<IPositionService>();

            await positionService.RecalculatePositionsAsync(rateEvent);
        }

        /// <summary>
        /// Handles position added events by adding the new position to the database.
        /// </summary>
        /// <param name="positionEvent"></param>
        /// <returns></returns>
        private async Task HandlePositionAdded(PositionAddedEvent positionEvent)
        {
            _logger.LogInformation($"Received position added event for {positionEvent.InstrumentId}");

            using var scope = _serviceProvider.CreateScope();
            var positionService = scope.ServiceProvider.GetRequiredService<IPositionService>();

            await positionService.AddPositionAsync(positionEvent);
        }

        /// <summary>
        /// Handles position closed events by updating the position status in the database.
        /// </summary>
        /// <param name="closedEvent"></param>
        /// <returns></returns>
        private async Task HandlePositionClosed(PositionClosedEvent closedEvent)
        {
            _logger.LogInformation($"Received position closed event for {closedEvent.PositionId}");

            using var scope = _serviceProvider.CreateScope();
            var positionService = scope.ServiceProvider.GetRequiredService<IPositionService>();

            await positionService.ClosePositionAsync(closedEvent);
        }

        /// <summary>
        /// Stops the event handler service, unsubscribing from all events on the message bus.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping event handler service");
            return Task.CompletedTask;
        }
    }
}

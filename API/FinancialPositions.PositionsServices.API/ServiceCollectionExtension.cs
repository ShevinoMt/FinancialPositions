using FinancialPositions.PositionsServices.Core.Repositories;
using FinancialPositions.PositionsServices.Repositories.EF;
using FinancialPositions.PositionsServices.Services;
using FinancialPositions.PositionsServices.Services.Core;
using FinancialPositions.Shared.Core.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace FinancialPositions.PositionsServices.API
{
    /// <summary>
    /// Dependency injection
    /// </summary>
    public static class ServiceCollectionExtension
    {

        /// <summary>
        /// Populate dependency injection
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configurationManager"></param>
        /// <returns></returns>
        public static IServiceCollection PopulateApiServices(this IServiceCollection services, ConfigurationManager configurationManager)
        {
            // Configure Entity Framework
            services.AddDbContext<PositionsDbContext>(options =>
            options.UseInMemoryDatabase("PositionsDb"));
            // Register repositories
            services.AddScoped<IPositionRepository, PositionRepository>();
            // Register services
            services.AddScoped<IPositionService, PositionService>();
            // Register messaging
            services.AddSingleton<IMessageBus, InMemoryMessageBus>();
            // Register hosted services
            services.AddHostedService<PositionsEventHandlerService>();

            return services;
        }
    }
}

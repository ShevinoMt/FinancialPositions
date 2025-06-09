using FinancialPositions.RatesService.Repositories.EF;
using FinancialPositions.RatesServices.Core.Repositories;
using FinancialPositions.RatesServices.Services.CoinMarketCap;
using FinancialPositions.RatesServices.Services.Core;
using FinancialPositions.Shared.Core.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace FinancialPositions.RatesServices.API
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
            services.AddDbContext<RatesDbContext>(options =>
                options.UseInMemoryDatabase("RatesDb"));

            // Register repositories
            services.AddScoped<IRateRepository, RateRepository>();
            // Register services
            services.AddHttpClient<IRatesService, CoinMarketCapService>();
            services.AddScoped<IRateProcessingService, RateProcessingService>();
            // Register messaging
            services.AddSingleton<IMessageBus, InMemoryMessageBus>();
            return services;
        }
    }
}

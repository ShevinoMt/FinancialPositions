using Microsoft.Extensions.Options;

namespace FinancialPositions.PositionsServices.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { Title = "Andre Cassar - Positions Service API", Version = "v1" });
                foreach (var name in Directory.GetFiles(AppContext.BaseDirectory, "*.xml", SearchOption.AllDirectories))
                {
                    c.IncludeXmlComments(name);
                }
            });

            builder.Services.PopulateApiServices(builder.Configuration);

            // Add logging
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Logger.LogInformation("Positions Service started on port {port}",
                app.Configuration["ASPNETCORE_URLS"] ?? "5002");

            app.Run();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using FinancialPositions.RatesServices.Core.Entities;

namespace FinancialPositions.RatesService.Repositories.EF
{
    public class RatesDbContext : DbContext
    {
        public RatesDbContext(DbContextOptions<RatesDbContext> options)
        : base(options) 
        {
        
        }

        public DbSet<Rate> Rates { get; set; }

        /// <summary>
        /// Configures the model for the Rate entity.
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Rate>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.Symbol, e.Timestamp });
                entity.Property(e => e.Value).HasPrecision(18, 8);
            });
        }
    }
}

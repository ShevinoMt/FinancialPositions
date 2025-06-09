using FinancialPositions.PositionsServices.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace FinancialPositions.PositionsServices.Repositories.EF
{
    public class PositionsDbContext : DbContext
    {
        public PositionsDbContext(DbContextOptions<PositionsDbContext> options)
        : base(options) 
        { 
        
        }
        
        public DbSet<Position> Positions { get; set; }

        /// <summary>
        /// Configures the model for the Position entity.
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Position>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.InstrumentId);
                entity.Property(e => e.Quantity).HasPrecision(18, 8);
                entity.Property(e => e.InitialRate).HasPrecision(18, 8);
                entity.Property(e => e.Side).HasConversion<string>();
            });
        }
    }
}

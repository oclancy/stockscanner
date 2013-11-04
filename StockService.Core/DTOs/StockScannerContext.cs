using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockService.Core.DTOs
{
    public class StockScannerContext : DbContext
    {
        public DbSet<Market> Markets { get; set; }
        //public DbSet<Sector> Sectors { get; set; }
        //public DbSet<Industry> Industries { get; set; }
        //public DbSet<CompanyStatistics> CompanyStatistics { get; set; }
        //public DbSet<StockQuote> StockQuote{ get; set; }
        public DbSet<Company> Companies { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<StockQuote>()
                .Property<DateTime>(s => s.LastUpdated)
                .HasColumnType("datetime2");

            modelBuilder.Entity<CompanyStatistics>()
                .Property<DateTime>(s => s.LastUpdated)
                .HasColumnType("datetime2");

            modelBuilder.Entity<Market>()
                        .HasMany<Sector>(s => s.Sectors)
                        .WithRequired(s => s.Market)
                        .HasForeignKey(s => s.MarketId)
                        .WillCascadeOnDelete();


            modelBuilder.Entity<Sector>()
                .HasMany<Industry>(s => s.Industries)
                .WithRequired(i => i.Sector)
                .HasForeignKey(i => i.SectorId)
                .WillCascadeOnDelete();


            modelBuilder.Entity<Industry>()
                .HasMany<Company>(c => c.Companies)
                .WithRequired(c => c.Industry)
                .HasForeignKey(c => c.IndustryId)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Company>()
                .HasOptional<CompanyStatistics>(c => c.CompanyStatistics);

            modelBuilder.Entity<Company>()
                .HasOptional<StockQuote>(c => c.StockQuote);

        }
    }
}

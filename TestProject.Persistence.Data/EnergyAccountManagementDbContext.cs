using Microsoft.EntityFrameworkCore;
using TestProject.Application.Interfaces;
using TestProject.Application.Models.Entities;

namespace TestProject.Persistence.Data
{
    public class EnergyAccountManagementDbContext : DbContext,IEnergyAccountManagementDbContext
    {

        public EnergyAccountManagementDbContext(DbContextOptions<EnergyAccountManagementDbContext> options) : base(options)
        {
          
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountMeterReading> AccountMeterReadings { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Seed();
        }
    }
}

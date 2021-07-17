
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Threading;
using System.Threading.Tasks;
using TestProject.Application.Models.Entities;

namespace TestProject.Application.Interfaces
{
    public interface IEnergyAccountManagementDbContext
    {
        DbSet<Account> Accounts { get; set; }

        DbSet<AccountMeterReading> AccountMeterReadings { get; set; }

        DatabaseFacade Database { get; }

        int SaveChanges();
        int SaveChanges(bool acceptAllChangesOnSuccess);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}

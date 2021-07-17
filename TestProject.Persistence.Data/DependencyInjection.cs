using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TestProject.Application.Constants;
using TestProject.Application.Interfaces;

namespace TestProject.Persistence.Data
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            string connStr = configuration.GetConnectionString(ConfigurationKeys.SQL_DB_CONN_STR);
            services.AddDbContext<IEnergyAccountManagementDbContext, EnergyAccountManagementDbContext>(options => options.UseSqlServer(configuration.GetConnectionString(ConfigurationKeys.SQL_DB_CONN_STR),
                b => b.MigrationsAssembly(typeof(EnergyAccountManagementDbContext).Assembly.FullName)
              ), ServiceLifetime.Scoped);

            return services;
        }
    }
}

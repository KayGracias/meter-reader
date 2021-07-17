using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TestProject.Application.Models.Entities;

namespace TestProject.Persistence.Data
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            string filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Resources\Test_Accounts.csv");
            List<Account> accounts = File.ReadAllLines(filePath)
                                  .Skip(1)
                                  .Select(v => GetAccount(v))
                                  .ToList();

            modelBuilder.Entity<Account>()
                .HasData(accounts);
        }

        public static Account GetAccount(string accountDetails)
        {
            var values = accountDetails.Split(",");
            return new Account
            {
                AccountId = int.Parse(values[0]),
                FirstName = values[1],
                LastName = values[2]
            };
        }
    }
}

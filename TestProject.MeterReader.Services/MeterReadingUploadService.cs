using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TestProject.Application.Interfaces;
using TestProject.Application.Models.Entities;
using TestProject.Application.Models.MeterReadingUpload;

namespace TestProject.MeterReader.Services
{
    public class MeterReadingUploadService : IMeterReadingUploadService
    {
        private int totalRecordsToProcess = 0;
        private int successfulReadings = 0;
        private int failedReadings = 0;
        private IEnergyAccountManagementDbContext _dbContext;


        public MeterReadingUploadService(IEnergyAccountManagementDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<MeterReadingUploadResponse> ProcessCustomerMeterReadingsAsync(IFormFile csvFile, CancellationToken cancellationToken)
        {
            try
            {
                var customerReadings = ParseCustomerMeterReadingFromCSV(csvFile);
                await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    foreach (var reading in customerReadings)
                    {
                        await _dbContext.AccountMeterReadings.AddRangeAsync(customerReadings);
                    }


                    await _dbContext.SaveChangesAsync(cancellationToken);
                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    //Some error in storing the records into db so return null;
                    return null;
                }
            }
            catch (Exception)
            {

                return null;
            }
           
            return new MeterReadingUploadResponse { FailedReadings = failedReadings, SuccessfulReadings = successfulReadings };
        }

        private List<AccountMeterReading> ParseCustomerMeterReadingFromCSV(IFormFile csvFile)
        {

            var meterReadingsAsStringList = new List<string>();

            //Read contents of the file
            using (var reader = new StreamReader(csvFile.OpenReadStream()))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    meterReadingsAsStringList.Add(line);
                    totalRecordsToProcess++;
                }

                //Read all lines except header 
                var customerReadings = meterReadingsAsStringList
                                      .Skip(1)
                                      .Select(r => GetCustomerMeterReading(r))
                                      .OfType<AccountMeterReading>()
                                      .ToList();


                //Get only distinct values
                var distictCustomerReadings = customerReadings.Select(x => x)
                    .DistinctBy(x => new { x.AccountID, x.MeterReadingDateTime, x.MeterReadValue }).ToList();


                if (distictCustomerReadings != null)
                {
                    successfulReadings = distictCustomerReadings.Count;
                }
                failedReadings = totalRecordsToProcess - successfulReadings - 1;


                return distictCustomerReadings;
            }
        }

        //Read csv string value, validate and convert to object
        public AccountMeterReading GetCustomerMeterReading(string meterReading)
        {
            try
            {
                AccountMeterReading customerMeterReading = new AccountMeterReading();
                var values = meterReading.Split(",");
                if (values.Count() < 3)
                {
                    return null;
                }
                else
                {
                    int accountId;
                    DateTimeOffset meterReadingDateTime;

                    //Validate Account Id
                    var isValidInteger = int.TryParse(values[0], out accountId);
                    if (!isValidInteger || !IsValidAccount(accountId))
                    {
                        return null;
                    }
                    customerMeterReading.AccountID = accountId;

                    //Validate Meter Reading date time
                    var isValidDateTime = DateTimeOffset.TryParse(values[1], out meterReadingDateTime);
                    if (!isValidDateTime)
                    {
                        return null;
                    }
                    customerMeterReading.MeterReadingDateTime = meterReadingDateTime;

                    if (IsValidMeterReading(values[2]))
                    {
                        customerMeterReading.MeterReadValue = values[2];
                    }
                    else
                    {
                        return null;
                    }
                    successfulReadings++;
                    return customerMeterReading;
                }
            }
            catch (System.Exception)
            {
                return null;
            }

        }

        public bool IsValidAccount(int accountId)
        {
            try
            {
                var account = _dbContext.Accounts.Where(x => x.AccountId == accountId).FirstOrDefault();
                return account != null ? true : false;
            }
            catch (Exception ex)
            {

                throw;
            }
         
        }

        public bool IsValidMeterReading(string meterReadValue)
        {
            Regex meterReadingPattern = new Regex("^[0-9]{5}$");
            return meterReadingPattern.IsMatch(meterReadValue);
        }


    }
}

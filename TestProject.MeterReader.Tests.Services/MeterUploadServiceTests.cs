using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NUnit.Framework;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestProject.Application.Models.Entities;
using TestProject.Application.Models.MeterReadingUpload;
using TestProject.MeterReader.Services;
using TestProject.Persistence.Data;

namespace TestProject.MeterReader.Tests.Services
{
    public class Tests
    {

        private EnergyAccountManagementDbContext _mockDbContext;
        private MeterReadingUploadService _meterReadingUploadService;
        private IFormFile mockCSVfile;

        [SetUp]
        public void Setup()
        {
            //Mock Database
            var options = new DbContextOptionsBuilder<EnergyAccountManagementDbContext>()
               .UseInMemoryDatabase(Guid.NewGuid().ToString());
            options.ConfigureWarnings(warnings => warnings.Log(InMemoryEventId.TransactionIgnoredWarning));

            _mockDbContext = new EnergyAccountManagementDbContext(options.Options);

            _meterReadingUploadService = new MeterReadingUploadService(_mockDbContext);

            CreateMockTestData();

        }

        private void CreateMockTestData()
        {
            _mockDbContext.Accounts.Add(new Account {AccountId = 1234, FirstName = "John", LastName = "Doe" });
            _mockDbContext.Accounts.Add(new Account { AccountId = 2345, FirstName = "Jack", LastName = "Glass" });
            _mockDbContext.Accounts.Add(new Account { AccountId = 5555, FirstName = "Fario", LastName = "Pinto" });
            _mockDbContext.SaveChangesAsync();

            string testData = $"AccountId,MeterReadingDateTime,MeterReadValue{Environment.NewLine}2344,08/05/2019 09:24,0X765{Environment.NewLine}1234,10/05/2019 09:24,23566,";
            mockCSVfile = GetFileMock("text/csv",testData,"test","test.csv");
        }

        private IFormFile GetFileMock(string contentType, string content, string name, string fileName)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(content);

            var file = new FormFile(
                baseStream: new MemoryStream(bytes),
                baseStreamOffset: 0,
                length: bytes.Length,
                name: name,
                fileName: fileName
            )
            {
                Headers = new HeaderDictionary(),
                ContentType = contentType
            };

            return file;
        }

        [TearDown]
        public void TearDown()
        {
            _mockDbContext.Dispose();
            _mockDbContext = null;
        }

        [Test]
        [TestCase(1234)]
        [TestCase(2345)]
        [TestCase(5555)]
        public void IsValidAccount_ShoulReturnTrueWhenValid(int accountId)
        {
            Assert.IsTrue(_meterReadingUploadService.IsValidAccount(accountId));
        }

        [Test]
        [TestCase(1238)]
        [TestCase(0)]
        public void IsValidAccount_ShoulReturnFalseWhenInValid(int accountId)
        {
            Assert.IsFalse(_meterReadingUploadService.IsValidAccount(accountId));
        }

        [Test]
        [TestCase("123422")]
        [TestCase("00")]
        [TestCase("ABCDS")]
        [TestCase("1234A")]
        [TestCase("")]
        public void IsValidMeterReading_ShoulReturnFalseWhenInValid(string meterReading)
        {
            Assert.IsFalse(_meterReadingUploadService.IsValidMeterReading(meterReading));
        }

        [Test]
        [TestCase("12342")]
        [TestCase("00001")]
        public void IsValidMeterReading_ShoulReturnTrueWhenValid(string meterReading)
        {
            Assert.IsTrue(_meterReadingUploadService.IsValidMeterReading(meterReading));
        }

        [Test]
        [TestCase("2347,22/04/2019 12:25,54")]//Invalid Account Id, Invalid Meter reading
        [TestCase("2347,22/04/2019 12:25,511114")]//Invalid Account Id, Invalid Meter reading
        [TestCase("2347,22/04/2019 12:25,AB123")]//Invalid Account Id, Invalid Meter reading
        [TestCase("1234,22/04/2019 12:25,6")] //valid Account Id, Invalid Meter reading
        public void GetCustomerMeterReading_shouldReturnNullIfError(string meterReadingRecord)
        {
            Assert.IsNull(_meterReadingUploadService.GetCustomerMeterReading(meterReadingRecord));
        }


        [Test]
        [TestCase("1234,22/04/2019 12:25,06875")] 
        public void GetCustomerMeterReading_shouldReturnObjectIfValidData(string meterReadingRecord)
        {
            var actualResult = _meterReadingUploadService.GetCustomerMeterReading(meterReadingRecord);
            Assert.IsNotNull(actualResult);
            Assert.AreEqual(1234, actualResult.AccountID);
            Assert.AreEqual(new DateTime(2019,04,22,12,25,00).ToShortDateString(), actualResult.MeterReadingDateTime.DateTime.ToShortDateString());
            Assert.AreEqual("06875", actualResult.MeterReadValue);
        }


        [Test]
        public async Task ProcessCustomerMeterReadingsAsync_shouldReturn1Successfuland1Failed()
        {
            MeterReadingUploadResponse expectedResponse = new MeterReadingUploadResponse { SuccessfulReadings = 1, FailedReadings = 1 };
            var actualResult = await _meterReadingUploadService.ProcessCustomerMeterReadingsAsync(mockCSVfile, new CancellationToken());
            Assert.IsNotNull(actualResult);
            Assert.AreEqual(expectedResponse.SuccessfulReadings, actualResult.SuccessfulReadings);
            Assert.AreEqual(expectedResponse.FailedReadings, actualResult.FailedReadings);
        }

        [Test]
        public async Task ProcessCustomerMeterReadingsAsync_shouldReturnNullIfFailed()
        {
            MeterReadingUploadService meterReadingUploadService = new MeterReadingUploadService(null);
            var actualResult = await meterReadingUploadService.ProcessCustomerMeterReadingsAsync(mockCSVfile, new CancellationToken());
            Assert.IsNull(actualResult);       
        }

    }
}
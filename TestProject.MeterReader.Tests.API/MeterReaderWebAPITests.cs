using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestProject.Application.Interfaces;
using TestProject.Application.Models;
using TestProject.Application.Models.MeterReadingUpload;
using TestProject.MeterReader.WebAPI.Controllers;

namespace TestProject.MeterReader.Tests.API
{
    public class Tests
    {
        
        private Mock<IEnergyAccountManagementDbContext> _mockDbContext;
        private Mock<IMeterReadingUploadService> _mockMeterReadingUploadService;

        [SetUp]
        public void Setup()
        {
            _mockDbContext = new Mock<IEnergyAccountManagementDbContext>();
            _mockMeterReadingUploadService = new Mock<IMeterReadingUploadService>();
            
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

        [Test]
        public void UploadMeterReading_Post_should_return_BadRequest_if_file_extension_non_csv()
        {
            string testData = "Random text file";
            var mockCSVfile = GetFileMock("text/plain", testData, "test", "test.txt");
            _mockMeterReadingUploadService.Setup(x => x.ProcessCustomerMeterReadingsAsync(It.IsAny<IFormFile>(), It.IsAny<CancellationToken>()))
                                           .Returns<MeterReadingUploadResponse>(null);
            MeterReadingController meterReadingController;
             meterReadingController = new MeterReadingController(_mockMeterReadingUploadService.Object);
            Task<IActionResult> actionResult = meterReadingController.UploadMeterReading(mockCSVfile, new CancellationToken());
            var result = actionResult.Result as BadRequestObjectResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ErrorResponse>(result.Value);
            var errorResponse = (ErrorResponse)result.Value;
            Assert.AreEqual("The file needs to be a valid csv file.", errorResponse.ErrorMessage);
        }


        [Test]
        public void UploadMeterReading_Post_should_return_BadRequest_if_some_error_in_processing()
        {
            string testData = "some,valid,csv,data";
            var mockCSVfile = GetFileMock("text/csv", testData, "test", "test.csv");
            MeterReadingController meterReadingController;
            meterReadingController = new MeterReadingController(_mockMeterReadingUploadService.Object);
            Task<IActionResult> actionResult = meterReadingController.UploadMeterReading(mockCSVfile, new CancellationToken());
            var result = actionResult.Result as BadRequestObjectResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ErrorResponse>(result.Value);
            var errorResponse = (ErrorResponse)result.Value;
            Assert.AreEqual("There was an error processing the provided file.", errorResponse.ErrorMessage);
        }


        [Test]
        public void UploadMeterReading_Post_should_return_Ok200_if_all_processing_successful()
        {
            var expectedResponse = new MeterReadingUploadResponse { SuccessfulReadings = 11, FailedReadings = 25 };
            string testData = "some,valid,csv,data";
            var mockCSVfile = GetFileMock("text/csv", testData, "test", "test.csv");
            _mockMeterReadingUploadService.Setup(x => x.ProcessCustomerMeterReadingsAsync(It.IsAny<IFormFile>(), It.IsAny<CancellationToken>()))
                                           .ReturnsAsync(expectedResponse);
            MeterReadingController meterReadingController;
            meterReadingController = new MeterReadingController(_mockMeterReadingUploadService.Object);
            Task<IActionResult> actionResult = meterReadingController.UploadMeterReading(mockCSVfile, new CancellationToken());
            var result = actionResult.Result as OkObjectResult;
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<MeterReadingUploadResponse>(result.Value);
            var actualResponse = (MeterReadingUploadResponse)result.Value;
            Assert.AreEqual(expectedResponse.SuccessfulReadings, actualResponse.SuccessfulReadings);
            Assert.AreEqual(expectedResponse.FailedReadings, actualResponse.FailedReadings);
            
        }


    }
}
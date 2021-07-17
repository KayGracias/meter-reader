using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using TestProject.Application.Interfaces;
using TestProject.Application.Models;
using TestProject.Application.Models.MeterReadingUpload;

namespace TestProject.MeterReader.WebAPI.Controllers
{
    [ApiController]
    [Route("meter-reading-uploads")]
    public class MeterReadingController : ControllerBase
    {
        private readonly IMeterReadingUploadService _meterReadingService;

        public MeterReadingController(IMeterReadingUploadService meterReadingService)
        {
            _meterReadingService = meterReadingService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(MeterReadingUploadResponse),StatusCodes.Status200OK)]
        public async Task<IActionResult> UploadMeterReading(IFormFile csvFile, CancellationToken cancellationToken=default)
        {
   
            var fileExtension = System.IO.Path.GetExtension(csvFile.FileName);
            if (fileExtension != ".csv")
            {
                return BadRequest(new ErrorResponse { ErrorMessage = "The file needs to be a valid csv file." });
            }
            var result = await _meterReadingService.ProcessCustomerMeterReadingsAsync(csvFile, cancellationToken);
            if (result != null)
            {
                return Ok(result);
            }

            return BadRequest(new ErrorResponse { ErrorMessage = "There was an error processing the provided file." });

        }


    }
}

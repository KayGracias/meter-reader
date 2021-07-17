using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Threading.Tasks;
using TestProject.Application.Models.MeterReadingUpload;

namespace TestProject.Application.Interfaces
{
    public interface IMeterReadingUploadService
    {
        Task<MeterReadingUploadResponse> ProcessCustomerMeterReadingsAsync(IFormFile csvFile, CancellationToken cancellationToken);
    }
}

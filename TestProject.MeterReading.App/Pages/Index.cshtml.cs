using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace TestProject.MeterReading.App.Pages
{
    public class IndexModel : PageModel
    {
        public const string APIBaseUrl = "https://localhost:44346/";

        [BindProperty]
        public IFormFile CSVFile { get; set; }

        public string Message { get; set; }


        public string Error { get; set; }
        public void OnGet()
        {

        }


        public async Task OnPostAsync()
        {
            //Validate file extension
            var fileExtension = Path.GetExtension(CSVFile.FileName);
            if (fileExtension != ".csv")
            {
                Error = "This is not a valid file extension. Please upload a csv file";
                return;

            }
            if (CSVFile.Length > 0)
            {
                try
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        //Get the file steam from the multiform data uploaded from the browser
                        await CSVFile.CopyToAsync(memoryStream);

                        using (var client = new HttpClient())
                        {
                            client.BaseAddress = new Uri(APIBaseUrl);
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.DefaultRequestHeaders.Accept.Add(
                                new MediaTypeWithQualityHeaderValue("multipart/form-data"));

                            //Build an multipart/form-data request to upload the file to Web API
                            using var form = new MultipartFormDataContent();
                            using var fileContent = new ByteArrayContent(memoryStream.ToArray());
                            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
                            form.Add(fileContent, "csvfile", CSVFile.FileName);
                            HttpResponseMessage response = await client.PostAsync("/meter-reading-uploads", form);
                            var result = await response.Content.ReadAsStringAsync();
                            if (response.IsSuccessStatusCode)
                            {
                                dynamic data = JObject.Parse(result);
                                Message = "Successful Records: " + data.successfulReadings + " \n Failed Readings: " + data.failedReadings;
                                return;
                            }

                        }
                    }
                }
                catch(HttpRequestException)
                {
                    Error = "Something went wrong while processing your request.";
                    return;
                }
                catch (Exception)
                {
                    //any other error has occured
                }

            }

            Error = "There was an error. Contact Administrator";

        }
    }
}

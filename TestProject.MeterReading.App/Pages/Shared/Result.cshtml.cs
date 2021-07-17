using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TestProject.MeterReading.App.Pages.Shared
{
    public class ResultModel : PageModel
    {
        public string Message { get; set; }
        public void OnGet(string resultMessage)
        {
            Message = resultMessage;
        }
    }
}

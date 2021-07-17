using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TestProject.MeterReading.App.Pages
{
    public class testModel : PageModel
    {
        public string message { get; set; }
        public void OnGet()
        {
            message = "this is a test message";
        }
    }
}

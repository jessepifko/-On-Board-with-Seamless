using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SeamlessLaunchpad.Models;

namespace SeamlessLaunchpad.Controllers
{
    public class LaunchpadController : Controller
    {
        private readonly SLPADDBContext _context;
        private static readonly string ApiKey;
        
        public LaunchpadController(SLPADDBContext context)
        {
            _context = context;
        }

        static LaunchpadController()
        {
            var keyStream = new StreamReader(System.IO.File.OpenRead("api.txt"));
            ApiKey = keyStream.ReadToEnd().Trim('\n');
            keyStream.Close();
        }

        // Gets first value in sequence or returns null
        public async Task<IActionResult> Index()
        {
            StartupListRootObject returnValue = (await Utilities.GetApiResponse<StartupListRootObject>("v0/appFo187B73tuYhyg", "Master List", "https://api.airtable.com", "api_key", ApiKey)).FirstOrDefault();
            return View(returnValue.Records);
        }

        public async Task<IActionResult> Index2()
        {
            FeedbackListRootObject test = (await Utilities.GetApiResponse<FeedbackListRootObject>("v0/appFo187B73tuYhyg", "Feedback", "https://api.airtable.com", "api_key", ApiKey)).FirstOrDefault();
            return View(test.Records);
        }

        public async Task<IActionResult> SuccessTest()
        {
            StartupListRootObject startupList = (await Utilities.GetApiResponse<StartupListRootObject>("v0/appFo187B73tuYhyg", "Master List", "https://api.airtable.com", "api_key", ApiKey)).FirstOrDefault();
            FeedbackListRootObject feedbackList = (await Utilities.GetApiResponse<FeedbackListRootObject>("v0/appFo187B73tuYhyg", "Feedback", "https://api.airtable.com", "api_key", ApiKey)).FirstOrDefault();
            ViewBag.StartupList = startupList.Records;
            ViewBag.FeedbackList = feedbackList.Records;
            return View();
        }

    }
}
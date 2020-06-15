using System;
using System.Collections.Generic;
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

        [HttpGet]
        public IActionResult AddStartup()
        {

            return View();
        }
        [HttpPost]
        public IActionResult AddStartup(Models.Startup newStartup)
        {
            if (ModelState.IsValid)
            {
                _context.Startup.Add(newStartup);
                _context.SaveChanges();
            }    
            return RedirectToAction("Index");
        }
        
        public IActionResult ViewDashboard()
        {
            var startups = _context.Startup.ToList();
            return View(startups);
        }

    }
}
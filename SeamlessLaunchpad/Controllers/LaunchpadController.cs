using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SeamlessLaunchpad.Models;
using SeamlessLaunchpad.ViewModel;

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
            //Grabbing the User's seamless Association 
            string id = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var thisUser = _context.AspNetUsers.FirstOrDefault(x => x.Id == id);

            //Sending list of startups and the favorites from DB and the user's association into the View
            var startups = _context.Startup.ToList();
            ViewStartupFavorite view = new ViewStartupFavorite() {
                StartupsToReview = startups,
                FavoriteStartups = _context.Favorites.Where(x => x.UserId == id).ToList<Favorites>(),
                UserAssociation = thisUser.Association
            };
            return View(view);
        }

        public IActionResult AddFavorite(int id)
        {
            Favorites newFavorite = new Favorites();
            newFavorite.UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            newFavorite.StartupId = id;

            if(ModelState.IsValid)
            {
                _context.Favorites.Add(newFavorite);
                _context.SaveChanges();
                return RedirectToAction("ViewDashboard");
            }
            else
            {
                return View();
            }
        }

        public IActionResult AddInterest(int id)
        {
            var startupToEdit = _context.Startup.Find(id);

            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var thisUser = _context.AspNetUsers.FirstOrDefault(x => x.Id == userId);

            startupToEdit.InterestedPartners += thisUser.Association;

            _context.Entry(startupToEdit).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.Update(startupToEdit);
            _context.SaveChanges();

            return RedirectToAction("ViewDashboard");

        }

        public IActionResult ViewSingle(int id)
        {
            var startupToView = _context.Startup.Find(id);
            return View(startupToView);
        }

    }
}
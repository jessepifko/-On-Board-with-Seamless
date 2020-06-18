using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SeamlessLaunchpad.Models;
using SeamlessLaunchpad.ViewModel;
using Startup = SeamlessLaunchpad.Models.Startup;


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
        [Authorize]
        public IActionResult Index()
        {
            //old cold pulling form airtable
            //StartupListRootObject returnValue = (await Utilities.GetApiResponse<StartupListRootObject>("v0/appFo187B73tuYhyg", "Master List", "https://api.airtable.com", "api_key", ApiKey)).FirstOrDefault();
            //return View(returnValue.Records);
            string id = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var thisUser = _context.AspNetUsers.FirstOrDefault(x => x.Id == id);
            return View(thisUser);

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

        [Authorize]
        public IActionResult ViewDashboard(string favOnly)
        {
            //Grabbing the User's seamless Association 
            string id = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var thisUser = _context.AspNetUsers.FirstOrDefault(x => x.Id == id);
            List<Favorites> favoriteStartups = _context.Favorites.Where(x => x.UserId == id).ToList<Favorites>();
            var startups = _context.Startup.Where(x => x.Status == null).ToList();

            //Making a list of "popularity" against startup ID
            Dictionary<int, int> startupFavoriteCount = new Dictionary<int, int>();
            List<Favorites> allFavorites = _context.Favorites.ToList<Favorites>();
            foreach (var s in startups)
            {
                int favCount = allFavorites.Where(x => x.StartupId == s.Id).Count();
                startupFavoriteCount.Add(s.Id, favCount);
            }

            List<KeyValuePair<int, int>> orderedStartupFavoriteCount = startupFavoriteCount.ToList().OrderBy(x => x.Value).Reverse().ToList();
            
            

            //Sending list of startups and the favorites from DB and the user's association into the View
            if (favOnly != null)
            {
                List<Models.Startup> onlyFavorites = new List<Models.Startup>();
                foreach (var s in startups)
                {
                    foreach (var f in favoriteStartups)
                    {
                        if (s.Id == f.StartupId)
                        {
                            onlyFavorites.Add(s);
                        }
                    }
                }

                FavoritesViewModel view = new FavoritesViewModel()
                {
                    StartupsToReview = onlyFavorites,
                    FavoriteStartups = favoriteStartups,
                    UserAssociation = thisUser.Association,
                    FavoriteCount = orderedStartupFavoriteCount
                };
                return View(view);
            }
            else
            {
                FavoritesViewModel view = new FavoritesViewModel() {
                    StartupsToReview = startups,
                    FavoriteStartups = favoriteStartups,
                    UserAssociation = thisUser.Association,
                    FavoriteCount = orderedStartupFavoriteCount
                };
                return View(view);
            }
            
        }
        [Authorize]
        public IActionResult AddFavorite(int id)
        {
            Favorites newFavorite = new Favorites();
            newFavorite.UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            newFavorite.StartupId = id;

            if (ModelState.IsValid)
            {
                _context.Favorites.Add(newFavorite);
                _context.SaveChanges();
                return RedirectToAction("ViewSingle", new { id = id });
            }
            else
            {
                return View();
            }
        }

        [Authorize]
        public IActionResult RemoveFavorite(int id)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var thisUser = _context.AspNetUsers.FirstOrDefault(x => x.Id == userId);

            var favoriteToRemove = _context.Favorites.Where(x => x.StartupId == id &&
                                                            x.UserId == userId).ToList();
            foreach (var f in favoriteToRemove)
            {
                _context.Favorites.Remove(f);
            }
            _context.SaveChanges();

            return RedirectToAction("ViewSingle", new { id = id });
        }

        [Authorize]
        public IActionResult AddInterest(int id)
        {
            var startupToEdit = _context.Startup.Find(id);

            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var thisUser = _context.AspNetUsers.FirstOrDefault(x => x.Id == userId);

            if (startupToEdit.InterestedPartners == "" || startupToEdit.InterestedPartners == null)
            {
                startupToEdit.InterestedPartners += thisUser.Association;
            }
            else
            {
                startupToEdit.InterestedPartners += ", " + thisUser.Association;
            }

            _context.Entry(startupToEdit).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.Update(startupToEdit);
            _context.SaveChanges();

            return RedirectToAction("ViewSingle", new { id = id });
        }

        [Authorize]
        public IActionResult RemoveInterest(int id)
        {
            var startupToEdit = _context.Startup.Find(id);
           
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var thisUser = _context.AspNetUsers.FirstOrDefault(x => x.Id == userId);

            string[] interestedPartnersArray = startupToEdit.InterestedPartners.Replace(" ", "").Split(',');
            string updatedInterestedPartnersString = "";

            foreach (string p in interestedPartnersArray)
            {
                if (p != thisUser.Association)
                {
                    if (updatedInterestedPartnersString == "")
                    {
                        updatedInterestedPartnersString = p;
                    }
                    else
                    {
                        updatedInterestedPartnersString += ", " + p;
                    }
                }
            }

            startupToEdit.InterestedPartners = updatedInterestedPartnersString;


            _context.Entry(startupToEdit).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.Update(startupToEdit);
            _context.SaveChanges();

            return RedirectToAction("ViewSingle", new { id = id });

        }

        [Authorize]
        public async Task<IActionResult> ViewSingle(int id)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var thisUser = _context.AspNetUsers.FirstOrDefault(x => x.Id == userId);
            //INSERT call to Mike's code here, MAYBE, that COULD return similar startups from the API and write them to a list of our Startup model 
            //OR we could add a list of API startup model to the viewmodel class, filter by what Mike's code returns, and pass that on...
            //INSERT call to Jess's code here, that will return an int for successViewStartupFavorite view = new ViewStartupFavorite() {

            List<PredictedApiStartup> topMatching = await CompareSuccess(id);


            FavoritesViewModel view = new FavoritesViewModel()
            {
                SingleStartupToView = _context.Startup.Find(id),
                FavoriteStartups = _context.Favorites.Where(x => x.UserId == userId).ToList<Favorites>(),
                UserAssociation = thisUser.Association,
                MatchingPredictedStartups = topMatching
            };

            return View(view);
        }

        [Authorize]
        [HttpGet]
        public IActionResult RemoveStartup(int id)
        {
            var startupToRemove = _context.Startup.Find(id);
            return View(startupToRemove);
        }
        [HttpPost]
        public IActionResult RemoveStartup(int id, string status)
        {
            var startupToRemove = _context.Startup.Find(id);

            startupToRemove.Status = status;
            startupToRemove.DateRemoved = DateTime.Now;

            _context.Entry(startupToRemove).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.Update(startupToRemove);
            _context.SaveChanges();

            return RedirectToAction("ViewDashboard");
        }

        [Authorize]
        [HttpGet]
        public IActionResult EditComments(int id)
        {
            var startupToEdit = _context.Startup.Find(id);
            return View(startupToEdit);
        }
        [HttpPost]
        public IActionResult EditComments(int id, string comments)
        {
            var startupToEdit = _context.Startup.Find(id);

            startupToEdit.Comments = comments;

            _context.Entry(startupToEdit).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.Update(startupToEdit);
            _context.SaveChanges();

            return RedirectToAction("ViewSingle", new { id = id });
        }


        public async Task<List<PredictedApiStartup>> CompareSuccess(int id)
        {
           //List<string> techAreasStrings = new List<string>();
            var startupToEdit = _context.Startup.Find(id);
            // List<ApiStartup> newList = new List<ApiStartup>();
            //var newList = startupToEdit;

            string[] techAreaStrings = startupToEdit.TechArea.Replace(" ", "").Split(',');
            StartupListRootObject startupList = (await Utilities.GetApiResponse<StartupListRootObject>("v0/appFo187B73tuYhyg", "Master List", "https://api.airtable.com", "api_key", ApiKey)).FirstOrDefault();
            StartupListRootObject filteredStartupList = new StartupListRootObject();
            filteredStartupList.Records = new List<StartupContainer>();
            foreach(string ta in techAreaStrings)
            {
                foreach(var record in startupList.Records)
                {
                    if (record.Fields.TechAreas != null)
                    {

                        if (record.Fields.TechAreas.Contains(ta))
                        {
                            filteredStartupList.Records.Add(record);
                        }
                    }
                }
            }

            FeedbackListRootObject feedbackList = (await Utilities.GetApiResponse<FeedbackListRootObject>("v0/appFo187B73tuYhyg", "Feedback", "https://api.airtable.com", "api_key", ApiKey)).FirstOrDefault();
            List<PredictedApiStartup> ratedFilteredApiStartups = new List<PredictedApiStartup>();
            for (int i = 0; i < filteredStartupList.Records.Count; i++)
            {
                ApiStartup singleStartup = (ApiStartup)filteredStartupList.Records[i].Fields;
                //PredictedApiStartup thisone = singleStartup as PredictedApiStartup;

                if (singleStartup != null)
                {
                    PredictedApiStartup pas = new PredictedApiStartup();
                    pas.CompanyName = singleStartup.CompanyName;
                    pas.CompanySummary = singleStartup.CompanySummary;
                pas.PredictedRating = SuccessPredictor.PredictSuccess(filteredStartupList.Records[i].Fields, feedbackList.Records);
                ratedFilteredApiStartups.Add(pas);

                }
            }

            ratedFilteredApiStartups = ratedFilteredApiStartups.OrderBy(x => x.PredictedRating).Reverse().ToList();
            List<PredictedApiStartup> topResults = new List<PredictedApiStartup>();
            for(int i = 0; i < 3; i++) //change i<# to change number of results
            {
               
                topResults.Add(ratedFilteredApiStartups[i]);
            }


            //    Dictionary<ApiStartup, int> kvp = new Dictionary<ApiStartup, int>();

            //foreach (StartupContainer l1 in startupList.Records)
            //{
            //   kvp.Add(l1.Fields, SuccessPredictor.PredictSuccess(l1.Fields, feedbackList.Records.Where(x => x.Fields.Startup.Equals(l1.Fields.CompanyName)).ToList()));
            //}
            return topResults;
        }


    }
}
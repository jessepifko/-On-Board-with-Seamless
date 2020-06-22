using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
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
        public IActionResult AddStartup(string name, string summary, string thegoodlife = "", string healthbeyondthehotpital = "",
            string robustfuture = "", string convenienceandproductivity = "", string softwareai = "", string sensing = "",
            string robotics = "", string products = "", string advancedmaterials = "", string businessprocess = "",
            string city = "", string country = "", string dateadded = "")
        {
            if(name == null)
            {
                return View();
            }

            Models.Startup startupToAdd = new Models.Startup();
            startupToAdd.Name = name;
            startupToAdd.Summary = summary;
            startupToAdd.City = city;
            startupToAdd.Country = country;
            string themes = "";

            if (thegoodlife != "")
            {
                themes += thegoodlife;
            } 
            if (healthbeyondthehotpital != "")
            {
                if(themes == "")
                {
                    themes += healthbeyondthehotpital;
                }
                else
                {
                    themes += ", " + healthbeyondthehotpital;
                }

            }
            if (robustfuture != "")
            {
                if (themes == "")
                {
                    themes += robustfuture;
                }
                else
                {
                    themes += ", " + robustfuture;
                }

            }
            if (convenienceandproductivity != "")
            {
                if (themes == "")
                {
                    themes += convenienceandproductivity;
                }
                else
                {
                    themes += ", " + convenienceandproductivity;
                }

            }
           
            startupToAdd.Theme = themes;

            string techArea = "";

            if (softwareai != "")
            {
                techArea += softwareai;
            }
            if (sensing != "")
            {
                if (techArea == "")
                {
                    techArea += sensing;
                }
                else
                {
                    techArea += ", " + sensing;
                }

            }
            if (robotics != "")
            {
                if (techArea == "")
                {
                    techArea += robotics;
                }
                else
                {
                    techArea += ", " + robotics;
                }

            }
            if (products != "")
            {
                if (techArea == "")
                {
                    techArea += products;
                }
                else
                {
                    techArea += ", " + products;
                }

            }
            if (advancedmaterials != "")
            {
                if (techArea == "")
                {
                    techArea += advancedmaterials;
                }
                else
                {
                    techArea += ", " + advancedmaterials;
                }

            }
            if (businessprocess != "")
            {
                if (techArea == "")
                {
                    techArea += businessprocess;
                }
                else
                {
                    techArea += ", " + businessprocess;
                }

            }

            startupToAdd.TechArea = techArea;

            startupToAdd.DateAdded = DateTime.Parse(dateadded);


            if (ModelState.IsValid)
            {
                _context.Startup.Add(startupToAdd);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpGet]
        public IActionResult ViewDashboard(string favOnly, string thegoodlife = "", string healthbeyondthehospital = "",
            string robustfuture = "", string convenienceproductivity = "", string softwareai = "", string sensing = "",
            string robotics = "", string products = "", string advancedmaterials = "", string businessprocess = "",
            string city = "", string country = "", string viewname = "")
        {
            //Grabbing the User's seamless Association 
            string id = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var thisUser = _context.AspNetUsers.FirstOrDefault(x => x.Id == id);
            List<Favorites> favoriteStartups = _context.Favorites.Where(x => x.UserId == id).ToList<Favorites>();
            var startups = _context.Startup.Where(x => x.Status == null).ToList();

            List<UserView> views = _context.UserView.Where(x => x.UserId.Equals(thisUser.Id)).ToList();
            ViewBag.UserViews = new List<UserView>();
            foreach (UserView v in views)
            {
                ViewBag.UserViews.Add(v);
            }
            //Sending list of startups and the favorites from DB and the user's association into the View
            if (favOnly == "yes")
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
                //Making a list of "popularity" against startup ID
                Dictionary<int, int> startupFavoriteCount = new Dictionary<int, int>();
                List<Favorites> allFavorites = _context.Favorites.ToList<Favorites>();
                foreach (var s in onlyFavorites)
                {
                    int favCount = allFavorites.Where(x => x.StartupId == s.Id).Count();
                    startupFavoriteCount.Add(s.Id, favCount);
                }

                List<KeyValuePair<int, int>> orderedStartupFavoriteCount = startupFavoriteCount.ToList().OrderBy(x => x.Value).Reverse().ToList();

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
                if (!string.IsNullOrEmpty(thegoodlife))
                {
                    startups = startups.Where(x => x.Theme.Contains("The Good Life")).ToList();
                    ViewBag.TheGoodLife = true;
                }
                if (!string.IsNullOrEmpty(healthbeyondthehospital))
                {
                    startups = startups.Where(x => x.Theme.Contains("Health Beyond the Hospital")).ToList();
                    ViewBag.HealthBeyondTheHospital = true;
                }
                if (!string.IsNullOrEmpty(robustfuture))
                {
                    startups = startups.Where(x => x.Theme.Contains("Robust Future")).ToList();
                    ViewBag.RobustFuture = true;
                }
                if (!string.IsNullOrEmpty(convenienceproductivity))
                {
                    startups = startups.Where(x => x.Theme.Contains("Convenience")).ToList();
                    ViewBag.ConvenienceProductivity = true;
                }
                if (!string.IsNullOrEmpty(softwareai))
                {
                    startups = startups.Where(x => x.TechArea.Contains("Software / AI")).ToList();
                    ViewBag.SoftwareAI = true;
                }
                if (!string.IsNullOrEmpty(sensing))
                {
                    startups = startups.Where(x => x.TechArea.Contains("Sensing")).ToList();
                    ViewBag.Sensing = true;
                }
                if (!string.IsNullOrEmpty(robotics))
                {
                    startups = startups.Where(x => x.TechArea.Contains("Robotics")).ToList();
                    ViewBag.Robotics = true;
                }
                if (!string.IsNullOrEmpty(products))
                {
                    startups = startups.Where(x => x.TechArea.Contains("Products")).ToList();
                    ViewBag.Products = true;
                }
                if (!string.IsNullOrEmpty(advancedmaterials))
                {
                    startups = startups.Where(x => x.TechArea.Contains("Advanced Materials")).ToList();
                    ViewBag.AdvancedMaterials = true;
                }
                if (!string.IsNullOrEmpty(businessprocess))
                {
                    startups = startups.Where(x => x.TechArea.Contains("Business Process")).ToList();
                    ViewBag.BusinessProcess = true;
                }
                if (!string.IsNullOrEmpty(city))
                {
                    startups = startups.Where(x => x.City != null && x.City.ToLower().Contains(city.ToLower())).ToList();
                    ViewBag.City = city;
                }
                if (!string.IsNullOrEmpty(country))
                {
                    startups = startups.Where(x => x.Country != null && x.Country.ToLower().Contains(country.ToLower())).ToList();
                    ViewBag.Country = country;
                }
                //Making a list of "popularity" against startup ID
                Dictionary<int, int> startupFavoriteCount = new Dictionary<int, int>();
                List<Favorites> allFavorites = _context.Favorites.ToList<Favorites>();
                foreach (var s in startups)
                {
                    int favCount = allFavorites.Where(x => x.StartupId == s.Id).Count();
                    startupFavoriteCount.Add(s.Id, favCount);
                }

                List<KeyValuePair<int, int>> orderedStartupFavoriteCount = startupFavoriteCount.ToList().OrderBy(x => x.Value).Reverse().ToList();

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
        [HttpGet]
        public async Task<IActionResult>GetSavedView(int selectedView)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            UserView view = _context.UserView.FirstOrDefault(x => x.Id == selectedView && x.UserId.Equals(userId));
            List<ViewFilter> filters = _context.ViewFilter.Where(y => y.ViewId == view.Id).ToList();
            bool first = true;
            string url = "/Launchpad/ViewDashboard";
            foreach(ViewFilter f in filters)
            {
                if (first)
                {
                    url += $"?{f.FilterName}={Uri.EscapeDataString(f.FilterValue)}";
                }
                else
                {
                    url += $"&{f.FilterName}={Uri.EscapeDataString(f.FilterValue)}";
                }
                first = false;
            }
            return Redirect(url);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> SaveView(string favOnly, string thegoodlife = "", string healthbeyondthehospital = "",
            string robustfuture = "", string convenienceproductivity = "", string softwareai = "", string sensing = "",
            string robotics = "", string products = "", string advancedmaterials = "", string businessprocess = "",
            string city = "", string country = "", string viewname = "")
        {
            if (string.IsNullOrEmpty(viewname))
            {
                return Forbid();
            }
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            UserView thisView = new UserView { UserId = userId, Name = viewname };
            _context.UserView.Add(thisView);
            await _context.SaveChangesAsync();
            if (!string.IsNullOrEmpty(thegoodlife))
            {
                ViewFilter filter = new ViewFilter { FilterName = "thegoodlife", FilterValue = "true", ViewId = thisView.Id };
                _context.ViewFilter.Add(filter);
            }
            if (!string.IsNullOrEmpty(healthbeyondthehospital))
            {
                ViewFilter filter = new ViewFilter { FilterName = "healthbeyondthehospital", FilterValue = "true", ViewId = thisView.Id };
                _context.ViewFilter.Add(filter);
            }
            if (!string.IsNullOrEmpty(robustfuture))
            {
                ViewFilter filter = new ViewFilter { FilterName = "robustfuture", FilterValue = "true", ViewId = thisView.Id };
                _context.ViewFilter.Add(filter);
            }
            if (!string.IsNullOrEmpty(convenienceproductivity))
            {
                ViewFilter filter = new ViewFilter { FilterName = "convenienceproductivity", FilterValue = "true", ViewId = thisView.Id };
                _context.ViewFilter.Add(filter);
            }
            if (!string.IsNullOrEmpty(softwareai))
            {
                ViewFilter filter = new ViewFilter { FilterName = "softwareai", FilterValue = "true", ViewId = thisView.Id };
                _context.ViewFilter.Add(filter);
            }
            if (!string.IsNullOrEmpty(sensing))
            {
                ViewFilter filter = new ViewFilter { FilterName = "sensing", FilterValue = "true", ViewId = thisView.Id };
                _context.ViewFilter.Add(filter);
            }
            if (!string.IsNullOrEmpty(robotics))
            {
                ViewFilter filter = new ViewFilter { FilterName = "robotics", FilterValue = "true", ViewId = thisView.Id };
                _context.ViewFilter.Add(filter);
            }
            if (!string.IsNullOrEmpty(products))
            {
                ViewFilter filter = new ViewFilter { FilterName = "products", FilterValue = "true", ViewId = thisView.Id };
                _context.ViewFilter.Add(filter);
            }
            if (!string.IsNullOrEmpty(advancedmaterials))
            {
                ViewFilter filter = new ViewFilter { FilterName = "advancedmaterials", FilterValue = "true", ViewId = thisView.Id };
                _context.ViewFilter.Add(filter);
            }
            if (!string.IsNullOrEmpty(businessprocess))
            {
                ViewFilter filter = new ViewFilter { FilterName = "businessprocess", FilterValue = "true", ViewId = thisView.Id };
                _context.ViewFilter.Add(filter);
            }
            if (!string.IsNullOrEmpty(city))
            {
                ViewFilter filter = new ViewFilter { FilterName = "city", FilterValue = city, ViewId = thisView.Id };
                _context.ViewFilter.Add(filter);
            }
            if (!string.IsNullOrEmpty(country))
            {
                ViewFilter filter = new ViewFilter { FilterName = "country", FilterValue = country, ViewId = thisView.Id };
                _context.ViewFilter.Add(filter);
            }
            await _context.SaveChangesAsync();
            return Redirect($"/Launchpad/ViewDashboard?favOnly={favOnly}&thegoodlife={thegoodlife}&healthbeyondthehospital={healthbeyondthehospital}" +
                $"&robustfuture={robustfuture}&convenienceproductivity={convenienceproductivity}&softwareai={softwareai}" +
                $"&sensing={sensing}&robotics={robotics}&products={products}&advancedmaterials{advancedmaterials}&businessprocess={businessprocess}" +
                $"&city={city}&country={country}");
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

        //[Authorize]
        //[HttpGet]
        //public IActionResult EditComments(int id)
        //{
        //    var startupToEdit = _context.Startup.Find(id);
        //    return View(startupToEdit);
        //}
        //[HttpPost]
        //public IActionResult EditComments(int id, string comments)
        //{
        //    var startupToEdit = _context.Startup.Find(id);

        //    startupToEdit.Comments = comments;

        //    _context.Entry(startupToEdit).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        //    _context.Update(startupToEdit);
        //    _context.SaveChanges();

        //    return RedirectToAction("ViewSingle", new { id = id });
        //}

        [Authorize]
        [HttpGet]
        public IActionResult AddComment(int id)
        {
            List<Comment> comments = _context.Comment.Where(x => x.StartupId == id).ToList<Comment>();
            ViewBag.StartupId = id;
            return View(comments);
        }
         [HttpPost]
        public IActionResult AddComment(int id, string comment)
        {
            return View();
        }




        public async Task<List<PredictedApiStartup>> CompareSuccess(int id)
        {
           //List<string> techAreasStrings = new List<string>();
            Models.Startup startupToEdit = _context.Startup.Find(id);
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
                        string apiTechAreas = record.Fields.TechAreas.Replace(" ", "").ToLower();
                        string thisTechArea = ta.Replace(" ", "").ToLower();
                        if (apiTechAreas.Contains(thisTechArea))
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
                try
                {
                    topResults.Add(ratedFilteredApiStartups[i]);
                }
                catch
                {
                    break;
                }
            }


            //    Dictionary<ApiStartup, int> kvp = new Dictionary<ApiStartup, int>();

            //foreach (StartupContainer l1 in startupList.Records)
            //{
            //   kvp.Add(l1.Fields, SuccessPredictor.PredictSuccess(l1.Fields, feedbackList.Records.Where(x => x.Fields.Startup.Equals(l1.Fields.CompanyName)).ToList()));
            //}
            return topResults;
        }

        public async Task<IActionResult> TestKeywordExtraction()
        {
            List<StartupContainer> sc = (await Utilities.GetApiResponse<StartupListRootObject>("v0/appFo187B73tuYhyg", "Master List", "https://api.airtable.com", "api_key", ApiKey)).FirstOrDefault().Records;
            List<StartupKeywords> lsk = await KeywordMatching.SetupKeywords(sc, _context);
            return View(lsk);
        }
    }
}
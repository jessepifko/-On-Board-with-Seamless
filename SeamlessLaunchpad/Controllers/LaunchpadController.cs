using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Final_Project_NewsApi_Testing.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualBasic;
using SeamlessLaunchpad.Models;
using SeamlessLaunchpad.ViewModel;
using Startup = SeamlessLaunchpad.Models.Startup;


namespace SeamlessLaunchpad.Controllers
{
    public class LaunchpadController : Controller
    {
        private readonly SLPADDBContext _context;
        private static readonly string ApiKey;
        private static readonly string NewsApiKey;
        private static readonly string RedlineApiKey;

        public LaunchpadController(SLPADDBContext context)
        {
            _context = context;
        }

        static LaunchpadController()
        {
            ApiKey = secret.apiKey;
        //    var keyStream = new StreamReader(System.IO.File.OpenRead("api.txt"));
        //    ApiKey = keyStream.ReadToEnd().Trim('\n');
            
        //    var newsKeyStream = new StreamReader(System.IO.File.OpenRead("newsapi.txt"));
            NewsApiKey = secret.newsApiKey;
            //    keyStream.Close();

          RedlineApiKey = secret.redLineApiKey;

        }

        // Gets first value in sequence or returns null
        [Authorize]
        public async Task<IActionResult> Index()
        {

            StartupListRootObject startupList = (await Utilities.GetApiResponse<StartupListRootObject>("v0/appFo187B73tuYhyg", "Master List", "https://api.airtable.com", "api_key", ApiKey)).FirstOrDefault();
            FeedbackListRootObject feedbackList = (await Utilities.GetApiResponse<FeedbackListRootObject>("v0/appFo187B73tuYhyg", "Feedback", "https://api.airtable.com", "api_key", ApiKey)).FirstOrDefault();
            ViewBag.StartupList = startupList.Records;
            ViewBag.FeedbackList = feedbackList.Records;
            //old cold pulling form airtable
            //StartupListRootObject returnValue = (await Utilities.GetApiResponse<StartupListRootObject>("v0/appFo187B73tuYhyg", "Master List", "https://api.airtable.com", "api_key", ApiKey)).FirstOrDefault();
            //return View(returnValue.Records);

            string id = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var thisUser = _context.AspNetUsers.FirstOrDefault(x => x.Id == id);
            return View(thisUser);


        }

        public IActionResult ViewLandingPage()
        {
            return View(); 
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
        public IActionResult AddStartup(string name, string summary, int teamscore, int uniqueness, string thegoodlife = "", string healthbeyondthehotpital = "",
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
            startupToAdd.TeamScore = teamscore;
            startupToAdd.UniqueScore = uniqueness;
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
            List<Models.Startup> startups = new List<Models.Startup>();

            List<UserView> views = _context.UserView.Where(x => x.UserId.Equals(thisUser.Id)).ToList();
            ViewBag.UserViews = new List<UserView>();
            try
            {
                ViewBag.ViewID = int.Parse(viewname);
            }
            catch
            {

            }
            foreach (UserView v in views)
            {
                ViewBag.UserViews.Add(v);
            }
            //Sending list of startups and the favorites from DB and the user's association into the View
            if (favOnly == "yes")
            {
                var startupsToView = _context.Startup.Where(x => x.Status == null).ToList();
                foreach (var s in startupsToView)
                {
                    foreach (var f in favoriteStartups)
                    {
                        if (s.Id == f.StartupId)
                        {

                            startups.Add(s);
                        }
                    }
                }
                
                ViewBag.HTW = "hide";
            }
            else
            {
                startups = _context.Startup.Where(x => x.Status == null).ToList();
            }
            if (!string.IsNullOrEmpty(thegoodlife))
            {
                startups = startups.Where(x => x.Theme.Contains("The Good Life")).ToList();
                ViewBag.TheGoodLife = true;
                ViewBag.HTW = "hide";
            }
            if (!string.IsNullOrEmpty(healthbeyondthehospital))
            {
                startups = startups.Where(x => x.Theme.Contains("Health Beyond the Hospital")).ToList();
                ViewBag.HealthBeyondTheHospital = true;
                ViewBag.HTW = "hide";
            }
            if (!string.IsNullOrEmpty(robustfuture))
            {
                startups = startups.Where(x => x.Theme.Contains("Robust Future")).ToList();
                ViewBag.RobustFuture = true;
                ViewBag.HTW = "hide";
            }
            if (!string.IsNullOrEmpty(convenienceproductivity))
            {
                startups = startups.Where(x => x.Theme.Contains("Convenience")).ToList();
                ViewBag.ConvenienceProductivity = true;
                ViewBag.HTW = "hide";
            }
            if (!string.IsNullOrEmpty(softwareai))
            {
                startups = startups.Where(x => x.TechArea.Contains("Software / AI")).ToList();
                ViewBag.SoftwareAI = true;
                ViewBag.HTW = "hide";
            }
            if (!string.IsNullOrEmpty(sensing))
            {
                startups = startups.Where(x => x.TechArea.Contains("Sensing")).ToList();
                ViewBag.Sensing = true;
                ViewBag.HTW = "hide";
            }
            if (!string.IsNullOrEmpty(robotics))
            {
                startups = startups.Where(x => x.TechArea.Contains("Robotics")).ToList();
                ViewBag.Robotics = true;
                ViewBag.HTW = "hide";
            }
            if (!string.IsNullOrEmpty(products))
            {
                startups = startups.Where(x => x.TechArea.Contains("Products")).ToList();
                ViewBag.Products = true;
                ViewBag.HTW = "hide";
            }
            if (!string.IsNullOrEmpty(advancedmaterials))
            {
                startups = startups.Where(x => x.TechArea.Contains("Advanced Materials")).ToList();
                ViewBag.AdvancedMaterials = true;
                ViewBag.HTW = "hide";
            }
            if (!string.IsNullOrEmpty(businessprocess))
            {
                startups = startups.Where(x => x.TechArea.Contains("Business Process")).ToList();
                ViewBag.BusinessProcess = true;
                ViewBag.HTW = "hide";
            }
            if (!string.IsNullOrEmpty(city))
            {
                startups = startups.Where(x => x.City != null && x.City.ToLower().Contains(city.ToLower())).ToList();
                ViewBag.City = city;
                ViewBag.HTW = "hide";
            }
            if (!string.IsNullOrEmpty(country))
            {
                startups = startups.Where(x => x.Country != null && x.Country.ToLower().Contains(country.ToLower())).ToList();
                ViewBag.Country = country;
                ViewBag.HTW = "hide";
            }
            Dictionary<int, int> startupFavoriteCount = new Dictionary<int, int>();
            List<Favorites> allFavorites = _context.Favorites.ToList<Favorites>();
            Dictionary<int, int> startupCommentCount = new Dictionary<int, int>();
            List<Comment> Comments = _context.Comment.ToList<Comment>();
            Dictionary<int, int> successPredictorScore = new Dictionary<int, int>();
            //FeedbackListRootObject feedbackList = (await Utilities.GetApiResponse<FeedbackListRootObject>("v0/appFo187B73tuYhyg", "Feedback", "https://api.airtable.com", "api_key", ApiKey)).FirstOrDefault();
            foreach (var s in startups)
            {
                int favCount = allFavorites.Where(x => x.StartupId == s.Id).Count();
                startupFavoriteCount.Add(s.Id, favCount);
                int comCount = Comments.Where(x => x.StartupId == s.Id &&
                                                x.Restricted == false).Count();
                startupCommentCount.Add(s.Id, comCount);
                //int success = SuccessPredictor.PredictSuccess(, feedbackList.Records);

            }

            List<KeyValuePair<int, int>> orderedStartupFavoriteCount = startupFavoriteCount.ToList().OrderBy(x => x.Value).Reverse().ToList();

            List<KeyValuePair<int, int>> orderedStartupCommentCount = startupCommentCount.ToList().OrderBy(x => x.Value).Reverse().ToList();


            ViewBag.Prediction = new List<int>();
            foreach (var startup in startups)
            {
                if (startup.TeamScore == null || startup.UniqueScore == null)
                {
                    ViewBag.Prediction.Add(0);
                    continue;
                }
                int prediction = NewStartupPredictor((int)startup.TeamScore, (int)startup.UniqueScore);

                ViewBag.Prediction.Add(prediction);
            }


            FavoritesViewModel view = new FavoritesViewModel()
            {
                StartupsToReview = startups,
                FavoriteStartups = favoriteStartups,
                UserAssociation = thisUser.Association,
                FavoriteCount = orderedStartupFavoriteCount,
                CommentCount = orderedStartupCommentCount
            };
            return View(view);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult>GetSavedView(int selectedView)
        {
            if (selectedView == 0)
            {
                return RedirectToAction("ViewDashboard");
            }
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            UserView view = _context.UserView.FirstOrDefault(x => x.Id == selectedView && x.UserId.Equals(userId));
            List<ViewFilter> filters = _context.ViewFilter.Where(y => y.ViewId == view.Id).ToList();
            string url = "/Launchpad/ViewDashboard?favOnly=no";
            foreach(ViewFilter f in filters)
            {
                url += $"&{f.FilterName}={Uri.EscapeDataString(f.FilterValue)}";
            }
            url += $"&viewname={selectedView}";
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
        public async Task<IActionResult> ViewSingle(int id, double distance = 0.0D )
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var thisUser = _context.AspNetUsers.FirstOrDefault(x => x.Id == userId);
            //INSERT call to Mike's code here, MAYBE, that COULD return similar startups from the API and write them to a list of our Startup model 
            //OR we could add a list of API startup model to the viewmodel class, filter by what Mike's code returns, and pass that on...
            //INSERT call to Jesse's code here, that will return an int for successViewStartupFavorite view = new ViewStartupFavorite() {

            List<PredictedApiStartup> topMatching = await CompareSuccess(id);

            Models.Startup startupToView = _context.Startup.Find(id);
            string nameToSearch = startupToView.Name.Replace(" ","+");
            NewsResult searchResult = (await Utilities.GetApiResponse<NewsResult>("v2", "everything", "https://newsapi.org", "apiKey", NewsApiKey, "q", nameToSearch)).FirstOrDefault();   //       "v0 /appFo187B73tuYhyg", "Master List", "https://api.airtable.com", "api_key", ApiKey)).FirstOrDefault();
            List<Article> articles = new List<Article>();
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    articles.Add(searchResult.articles[i]);
                } 
                catch
                {
                    continue;
                }
            }
                
            
            FavoritesViewModel view = new FavoritesViewModel()
            {
                SingleStartupToView = startupToView,
                FavoriteStartups = _context.Favorites.Where(x => x.UserId == userId).ToList<Favorites>(),
                UserAssociation = thisUser.Association,
                MatchingPredictedStartups = topMatching,
                Comments = _context.Comment.Where(x => x.StartupId == id &&
                                                    x.Restricted == false).ToList<Comment>(),
                ExclusiveComments = _context.Comment.Where(x => x.StartupId == id &&
                                                           x.Restricted == true &&
                                                           x.Association == thisUser.Association).ToList<Comment>(),
                Articles = articles
                                                           
            };

            ViewBag.Distance = distance;

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
        public IActionResult AddComment(int id)
        {
            List<Comment> comments = _context.Comment.Where(x => x.StartupId == id && 
                                                            x.Restricted == false).ToList<Comment>();
            ViewBag.StartupId = id;
            return View(comments);
        }
         [HttpPost]
        public IActionResult AddComment(int id, string comment)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var thisUser = _context.AspNetUsers.FirstOrDefault(x => x.Id == userId);
            
            Comment commentToAdd = new Comment();
            commentToAdd.StartupId = id;
            commentToAdd.UserName = thisUser.UserName;
            commentToAdd.Association = thisUser.Association;
            commentToAdd.CommentDate = DateTime.Now;
            commentToAdd.Comment1 = comment;
            commentToAdd.Restricted = false;

            if (ModelState.IsValid)
            {
                _context.Comment.Add(commentToAdd);
                _context.SaveChanges();
            }
            return RedirectToAction("ViewSingle", new { id = id });
        }

        [Authorize]
        [HttpGet]
        public IActionResult AddExclusiveComment(int id)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var thisUser = _context.AspNetUsers.FirstOrDefault(x => x.Id == userId);

            List<Comment> comments = _context.Comment.Where(x => x.StartupId == id &&
                                                            x.Association == thisUser.Association &&
                                                            x.Restricted == true).ToList<Comment>();
            ViewBag.StartupId = id;
            ViewBag.UserAssoc = thisUser.Association;
            return View(comments);
        }
        [HttpPost]
        public IActionResult AddExclusiveComment(int id, string comment)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var thisUser = _context.AspNetUsers.FirstOrDefault(x => x.Id == userId);

            Comment commentToAdd = new Comment();
            commentToAdd.StartupId = id;
            commentToAdd.UserName = thisUser.UserName;
            commentToAdd.Association = thisUser.Association;
            commentToAdd.CommentDate = DateTime.Now;
            commentToAdd.Comment1 = comment;
            commentToAdd.Restricted = true;

            if (ModelState.IsValid)
            {
                _context.Comment.Add(commentToAdd);
                _context.SaveChanges();
            }
            return RedirectToAction("ViewSingle", new { id = id });
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

            return topResults;
        }

        public int NewStartupPredictor(int teamScore, int uniqueness)
        {
           int teamSuccess = 0; 
            if (teamScore == 5)
            {
                teamSuccess = 5;
            }
            else if (teamScore == 4)
            {
                teamSuccess = 4;
            }
            else if (teamScore == 3)
            {
                teamSuccess = 3;
            }
            else if (teamScore == 2)
            {
                teamSuccess = 2; 
            }
            else if (teamScore == 1)
            {
                teamSuccess = 1;
            }


            int Uni = 0;
            if (uniqueness == 5)
            {
                Uni = 5;
            }
            else if (uniqueness == 4)
            {
                Uni = 4;
            }
            else if (uniqueness == 3)
            {
                Uni = 3;
            }
            else if (uniqueness == 2)
            {
                Uni = 2;
            }
            else if (uniqueness == 1)
            {
                Uni = 1; 
            }

            int newScore = (Uni + teamSuccess) / 2;
            return newScore;

        }

        public async Task<IActionResult> Distance(int id, int startzip, int endzip)
        {
            DistanceModel dis = (await Utilities.GetApiResponse<DistanceModel>($"rest/distance.json/{startzip:D5}/{endzip:D5}", "mile", "https://redline-redline-zipcode.p.rapidapi.com", "rapidapi-key", RedlineApiKey)).FirstOrDefault();

            if (dis != null )
            {
                ViewBag.Distance = dis.Distance;
            }
            return RedirectToAction("ViewSingle", new { id = id, distance = dis.Distance});
        }

    }
}
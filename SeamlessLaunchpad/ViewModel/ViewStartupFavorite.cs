using SeamlessLaunchpad.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeamlessLaunchpad.ViewModel
{
    public class ViewStartupFavorite
    {
        public List<Models.Startup> StartupsToReview { get; set; }
        
        public Models.Startup SingleStartupToView { get; set; }
        public List<Favorites> FavoriteStartups { get; set; }
        public string UserAssociation { get; set; }

        public int PredictedSuccess { get; set; }
    }
}

using SeamlessLaunchpad.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeamlessLaunchpad.ViewModel
{
    public class FavoritesViewModel
    {
        public List<Models.Startup> StartupsToReview { get; set; }

        public Models.Startup SingleStartupToView { get; set; }
        public List<Favorites> FavoriteStartups { get; set; }
        public string UserAssociation { get; set; }

        public int PredictedSuccess { get; set; }

        public List<KeyValuePair<int, int>> FavoriteCount { get; set; }

        public List<KeyValuePair<int, int>> CommentCount { get; set; }


        public List<PredictedApiStartup> MatchingPredictedStartups { get; set; }

        public List<Comment> ExclusiveComments { get; set; }

        public List<Comment> Comments { get; set; }



    }
}

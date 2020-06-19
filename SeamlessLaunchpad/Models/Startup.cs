using System;
using System.Collections.Generic;

namespace SeamlessLaunchpad.Models
{
    public partial class Startup
    {
        public Startup()
        {
            Favorites = new HashSet<Favorites>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Theme { get; set; }
        public string TechArea { get; set; }
        public string Summary { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public DateTime? DateAdded { get; set; }
        public string Comments { get; set; }
        public string InterestedPartners { get; set; }
        public DateTime? DateRemoved { get; set; }
        public string Status { get; set; }
        public int? TeamScore { get; set; }
        public int? UniqueScore { get; set; }

        public virtual ICollection<Favorites> Favorites { get; set; }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SeamlessLaunchpad.Models
{
    public class Startups
    {
        [JsonProperty("Company Name")]
        public string CompanyName { get; set; }
        [JsonProperty("Date Added")]
        public DateTime DateAdded { get; set; }
        public string Scout { get; set; }
        public string Source { get; set; }
        [JsonProperty("Company Website")]
        public string CompanyWebsite { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        [JsonProperty("Two Line Company Summary")]
        public string CompanySummary { get; set; }
        public string Alignment { get; set; }
        [JsonProperty("Theme(s)")]
        public string Theme { get; set; }
        public int Uniqueness { get; set; }
        public int Team { get; set; }
        public string Raised { get; set; }

        
    }
}

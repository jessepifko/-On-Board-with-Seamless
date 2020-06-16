using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeamlessLaunchpad.Models
{
    public class Feedback
    {
        public string Questions { get; set; }
        public string Startup { get; set; }
        [JsonProperty("Team Strength")]
        public int TeamStrength { get; set; }

    }
}

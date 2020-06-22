using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeamlessLaunchpad.Models
{
    public class Keyword
    {
        [JsonProperty("keyword")]
        public string Text { get; set; }
        [JsonProperty("confidence_score")]
        public double Confidence { get; set; }
    }
}

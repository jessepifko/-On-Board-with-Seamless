using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeamlessLaunchpad.Models
{
    public class FeedbackContainer
    {
        public string Id { get; set; }

        public Feedback Fields { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}

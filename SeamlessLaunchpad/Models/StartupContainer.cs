using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeamlessLaunchpad.Models
{
    public class StartupContainer
    {
        public string Id { get; set; }

        public Startups Fields { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}

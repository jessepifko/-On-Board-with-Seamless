using System;
using System.Collections.Generic;

namespace SeamlessLaunchpad.Models
{
    public partial class Favorites
    {
        public int Id { get; set; }
        public int? StartupId { get; set; }
        public string UserId { get; set; }

        public virtual Startup Startup { get; set; }
        public virtual AspNetUsers User { get; set; }
    }
}

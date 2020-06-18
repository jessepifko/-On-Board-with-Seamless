using System;
using System.Collections.Generic;

namespace SeamlessLaunchpad.Models
{
    public partial class UserView
    {
        public UserView()
        {
            ViewFilter = new HashSet<ViewFilter>();
        }

        public int Id { get; set; }
        public string UserId { get; set; }

        public virtual AspNetUsers User { get; set; }
        public virtual ICollection<ViewFilter> ViewFilter { get; set; }
    }
}

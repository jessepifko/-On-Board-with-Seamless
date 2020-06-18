using System;
using System.Collections.Generic;

namespace SeamlessLaunchpad.Models
{
    public partial class ViewFilter
    {
        public int Id { get; set; }
        public string FilterName { get; set; }
        public string FilterValue { get; set; }
        public int ViewId { get; set; }

        public virtual UserView View { get; set; }
    }
}

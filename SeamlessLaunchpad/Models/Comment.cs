using System;
using System.Collections.Generic;

namespace SeamlessLaunchpad.Models
{
    public partial class Comment
    {
        public int Id { get; set; }
        public int StartupId { get; set; }
        public string UserName { get; set; }
        public string Association { get; set; }
        public bool? Restricted { get; set; }
        public DateTime? CommentDate { get; set; }
        public string Comment1 { get; set; }

        public virtual Startup Startup { get; set; }
    }
}

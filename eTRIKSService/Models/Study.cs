using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eTRIKSService.Models
{
    public class Study
    {
        public string OID { get; set; }
        public string name { get; set; }
        public string description { get; set; }

        public virtual ICollection<Activity> Activity_TAB { get; set; }
    }
}
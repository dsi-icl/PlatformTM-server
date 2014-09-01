using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eTRIKS.Commons.Core.Domain.Model
{
    public class Study : Identifiable
    {
        //public string OID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<Activity> Activities { get; set; }
    }
}
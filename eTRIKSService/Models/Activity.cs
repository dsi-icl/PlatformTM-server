using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eTRIKSService.Models
{
    public class Activity
    {
        public string OID { get; set; }
        public string name { get; set; }
        public string studyId { get; set; }

        public virtual ICollection<Dataset> activityDatasets { get; set; }
        public virtual Study Study { get; set; }
    }
}
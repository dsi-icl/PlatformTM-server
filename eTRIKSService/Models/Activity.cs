using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eTRIKSService.Models
{
    public class Activity
    {
        public string OID { get; set; }
        public string activityName { get; set; }

        public DomainVariable dsm;
    }
}
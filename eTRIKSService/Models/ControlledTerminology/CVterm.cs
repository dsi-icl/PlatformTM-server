using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eTRIKSService.Models.ControlledTerminology
{
    public class CVterm
    {
        public string abbrv { get; set; }
        public string name { get; set; }
        public int order { get; set; }
        public int rank { get; set; }
        public bool userSpecified { get; set; }

    }
}
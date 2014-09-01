using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eTRIKS.Commons.Core.Domain.Model.ControlledTerminology
{
    public class Dictionary
    {
        public string OID { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public string URI { get; set; }
        public virtual ICollection<CVterm> ControlledTerms { get; set; }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eTRIKS.Commons.Core.Domain.Model.ControlledTerminology
{
    public class Dbxref
    {
        public string OID { get; set; }
        public string Accession { get; set; }
        public string Description { get; set; }

        public string DBId { get; set; }
        public DB DB { get; set; }
    }
}
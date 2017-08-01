using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eTRIKS.Commons.Service.Services.OntologyManagement
{
    public class Ontology
    {
        public string OntologyID { get; set; }
        public string OntologyVersion { get; set; }
        public string OntologyDisplayLabel { get; set; }
        public string OntologyAbbreviation {get; set; }
        public string SubmissionId { get; set; }

        //private String contactName, contactEmail, homepage;
    }
}

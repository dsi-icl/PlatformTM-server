using System;
using System.Collections.Generic;
using PlatformTM.Core.Domain.Model.ControlledTerminology;

namespace PlatformTM.Services.Services.OntologyManagement
{
    public class OLSclient : IOntologyService
    {
        private OLSresultHandler _resultHandler;
        public OLSclient(OLSresultHandler resultHandler)
        {
            _resultHandler = resultHandler;
        }

        public CVterm GetTerm(string termId, string ontology)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, CVterm> GetTermChildren(string termAccession, string ontology)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, List<CVterm>> ExactSearch(string term, string ontology)
        {
            throw new NotImplementedException();
        }

        public List<Ontology> GetAllOntologies()
        {
            throw new NotImplementedException();
        }
    }
}

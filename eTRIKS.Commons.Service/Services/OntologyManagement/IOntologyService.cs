﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Model.ControlledTerminology;

namespace eTRIKS.Commons.Service.Services.OntologyManagement
{
    public interface IOntologyService
    {
        CVterm GetTerm(string termId, string ontology);

        /**
         * Return the children of a term
         *
         * @param termAccession - term to search under
         * @param ontology      - ontology term is located in
         * @return Map<String,String> with mappings from term accession to the term label
         */
        Dictionary<string, CVterm> GetTermChildren(string termAccession, string ontology);

        Dictionary<string, List<CVterm>> ExactSearch(string term, string ontology);

        List<Ontology> GetAllOntologies();
    }
}
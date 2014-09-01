using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eTRIKSService.DTO
{
    public class DomainDataset
    {
        public string OID { get; set; }
        public string domainDatasetName { get; set; }
        public string domainDatasetClass { get; set; }
        public string description { get; set; }
        public string structure { get; set; }
        public Nullable<bool> repeating { get; set; }

        public List<DTO.DomainVariable> variableList = new List<DomainVariable>();
    }
}
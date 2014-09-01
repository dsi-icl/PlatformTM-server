using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eTRIKSService.Models.ControlledTerminology;

namespace eTRIKSService.Models
{
    public class VariableDefinition
    {
       
        public string variableId { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string dataType { get; set; }
        public Nullable<bool> isCurated { get; set; }
        public CVterm variableType { get; set; }
        public CVterm role { get; set; }

        public string datasetId { get; set; }
        public string studyId { get; set; }

      
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eTRIKS.Commons.Core.Domain.Model.Curation
{
    public class VariableLevelMetadata
    {
        // if a variable is mapped then itt will be assiociated with this class to be used for tabulation
        public string Variable { get; set; }      // Name of Variable
        public string Domain { get; set; }
        public string VarNum { get; set; }        // The order of the variable
        public string Type { get; set; }          // should be text, integer, float, datetime, date or time
        public string Length { get; set; }
        public string Lable { get; set; }         // The label of the SDTM variable as dictated by CDISC.
        public string Origin { get; set; }
        public string CodelistName { get; set; }
        public string Mandatory { get; set; }
        public string Role { get; set; }
        
    }
}

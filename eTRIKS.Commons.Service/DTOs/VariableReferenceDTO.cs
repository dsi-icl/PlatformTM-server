using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTRIKS.Commons.Service.DTOs
{
    public class VariableReferenceDTO
    {
        public string OrderNumber { get; set; }
        public string IsRequired { get; set; }
        public string KeySequence { get; set; }

        public string VariableDefinition { get; set; }
        public string VariableDefinitionId { get; set; }

        public string Dataset { get; set; }
        public string DatasetId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTRIKS.Commons.Service.DTOs
{
    public class DataTemplateMap
    {
        public string Domain { get; set; }
        public string ObservationName { get; set; }
        public Dictionary<string,List<VariableMap>> VarTypes{ get; set; }
        public List<string> TopicColumns { get; set; }


        public class VariableMap
        {
            public string Label { get; set; }
            public string Description { get; set; }
            public string ShortName { get; set; }
            public List<ColHeaderDTO> MapToColList { get; set; }
            public string DataType { get; set; }
            public List<string> MapToStringValueList { get; set; }
 
        }
    }
}

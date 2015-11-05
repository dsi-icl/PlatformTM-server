using System;
using System.Collections;
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
        //public List<Dictionary<string,List<VariableMap>>> VarTypes{ get; set; }
        public List<VariableType> VarTypes { get; set; } 
        public List<string> TopicColumns { get; set; }

        public DataTemplateMap()
        {
            TopicColumns = new List<string>(){"1"};
        }

        public class VariableType
        {
            public string name { get; set; }
            public List<VariableMap> vars { get; set; }
        }

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Core.Domain.Model.Templates;

namespace eTRIKS.Commons.Core.Domain.Model.Data.SDTM
{
    public class SdtmRowDescriptor
    {
        public string DomainName { get; set; }
        public string DomainCode { get; set; }
        public string Class { get; set; }
        public VariableDefinition O3Variable { get; set; } //VSTESTCD // AETERM // CMTRT
        public VariableDefinition O3SynoymVariable { get; set; } //--TEST //--MODIFY
        public VariableDefinition O3CVterm { get; set; } //--LOINC // --DECOD

        //public List<VariableDefinition> GroupDescriptors { get; set; }
        public VariableDefinition GroupVariable { get; set; }
        public VariableDefinition SubgroupVariable { get; set; }
        public List<VariableDefinition> SynonymVariables { get; set; }
        public List<VariableDefinition> VariableQualifierVariables { get; set; }
        public List<VariableDefinition> ResultVariables { get; set; }
        public List<VariableDefinition> TimeDescriptors { get; set; }
        public List<VariableDefinition> QualifierVariables { get; set; } //same catergory as DefaultQualifier ... still not sure about the name
        public VariableDefinition DefaultQualifier { get; set; }//AspectOfObservationDescriptor//MeasureOfObservationDescriptor
        //public VariableDefinition FindingsResultVariable { get; set; }
        //public int DefaultQualifierId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Core.Domain.Model.Templates;

namespace eTRIKS.Commons.Core.Domain.Model
{
    public class SdtmEntityDefine
    {
        public string DomainName { get; set; }
        public DomainTemplate Domain { get; set; }
        public VariableDefinition O3Descriptor { get; set; } //VSTESTCD //
        public VariableDefinition ControlledTermVariable { get; set; }
        public int O3DescriptorId { get; set; }

        public List<VariableDefinition> GroupDescriptors { get; set; }
        public List<VariableDefinition> SynonymVariables { get; set; }
        public List<VariableDefinition> VariableQualifierVariables { get; set; }
        public List<VariableDefinition> ResultVariables { get; set; }
        public List<VariableDefinition> TimeDescriptors { get; set; }
        public List<VariableDefinition> QualifierVariables { get; set; } //same catergory as DefaultQualifier ... still not sure about the name
        
        
        public VariableDefinition DefaultQualifier { get; set; }//AspectOfObservationDescriptor//MeasureOfObservationDescriptor
        public int DefaultQualifierId { get; set; }
    }
}

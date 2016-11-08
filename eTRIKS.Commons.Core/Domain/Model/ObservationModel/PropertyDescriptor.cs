using System.Collections.Generic;
using eTRIKS.Commons.Core.Domain.Model.Base;
using eTRIKS.Commons.Core.Domain.Model.ControlledTerminology;

namespace eTRIKS.Commons.Core.Domain.Model.Data
{
    public class PropertyDescriptor : Identifiable<int>
    {
        //public VariableReference DatasetVariable { get; set; }
        public DescriptorType Type { get; set; }
        public ObsValueType ValueType { get; set; }
        public string Name { get; set; }
        public CVterm CVterm { get; set; }
        public string Description { get; set; }
        public string ObsClass { get; set; }
        public List<PropertyDescriptor> RelDescriptors { get; set; } //UNIT
        public Project Project { get; set; }
        public int ProjectId { get; set; }
    }
}

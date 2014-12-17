using eTRIKS.Commons.Core.Domain.Model.Base;
using eTRIKS.Commons.Core.Domain.Model.ControlledTerminology;
using System.Runtime.Serialization;

namespace eTRIKS.Commons.Core.Domain.Model.Templates
{
    public class DomainVariableTemplate : Identifiable<string>
    {
        //public string OID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string DataType { get; set; }
        public string Label { get; set; }
        public int Order { get; set; }
        public string DomainId { get; set; }
        public string VariableTypeId { get; set; }
        public string RoleId { get; set; }
        public string UsageId { get; set; }
        public string controlledTerminologyId { get; set; }

        public  CVterm VariableType { get; set; }
        public CVterm Role { get; set; }
        public CVterm Usage { get; set; }
        public  DomainTemplate Domain { get; set; }
        public  Dictionary controlledTerminology { get; set; }
    }
}
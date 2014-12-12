using eTRIKS.Commons.Core.Domain.Model.Base;
using eTRIKS.Commons.Core.Domain.Model.ControlledTerminology;
using System.Runtime.Serialization;

namespace eTRIKS.Commons.Core.Domain.Model.Templates
{
    //[KnownType(typeof(DomainDataset))]
    //[DataContract]
    public class DomainTemplateVariable : Identifiable<string>
    {
        //public string OID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string DataType { get; set; }
        public string Label { get; set; }
        public string DomainId { get; set; }
        public string VariableTypeId { get; set; }
        public string RoleId { get; set; }
        public string UsageId { get; set; }
        public string controlledTerminologyId { get; set; }

        public virtual CVterm VariableType { get; private set; }
        public virtual CVterm Role { get; private set; }
        public virtual CVterm Usage { get; private set; }
        public virtual DomainDataset Domain { get; private set; }
        public virtual Dictionary controlledTerminology { get; private set; }
    }
}
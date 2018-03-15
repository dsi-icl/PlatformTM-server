namespace PlatformTM.Core.Domain.Model.DatasetModel
{
    public class VariableReference
    { 
        public int? OrderNumber { get; set; }
        public bool? IsRequired { get; set; }
        public int? KeySequence { get; set; }

        public VariableDefinition VariableDefinition { get; set; }
        public int VariableDefinitionId { get; set; }

        public Dataset Dataset { get; private set; }
        public int DatasetId { get; set; }
    }
}
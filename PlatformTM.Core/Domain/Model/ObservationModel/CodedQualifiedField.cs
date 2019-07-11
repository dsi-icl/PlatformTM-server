using System;
using PlatformTM.Core.Domain.Model.ControlledTerminology;
using PlatformTM.Core.Domain.Model.DatasetModel;

namespace PlatformTM.Core.Domain.Model.ObservationModel
{
    public class CodedQualifiedField
    {
        public VariableDefinition TermField { get; set; } //VSTESTCD // AETERM // CMTRT
        public VariableDefinition CodeField { get; set; } //--TEST //--MODIFY
        public Dictionary Vocabulary { get; set; } //--LOINC // --DECOD
        public CodedQualifiedField()
        {
        }
    }
}

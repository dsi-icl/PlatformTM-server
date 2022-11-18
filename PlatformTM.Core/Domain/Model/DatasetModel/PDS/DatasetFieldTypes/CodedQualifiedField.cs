
using PlatformTM.Core.Domain.Model.ControlledTerminology;
using PlatformTM.Core.Domain.Model.DatasetModel.PDS.DatasetFieldTypes;

namespace PlatformTM.Core.Domain.Model.PDS
{
    public class CodedQualifiedField
    {
        public DatasetField TermField { get; set; } //VSTESTCD // AETERM // CMTRT
        public DatasetField CodeField { get; set; } //--TEST //--MODIFY
        public Dictionary Vocabulary { get; set; } //--LOINC // --DECOD
        public CodedQualifiedField()
        {
        }
    }
}

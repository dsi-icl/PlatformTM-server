using eTRIKS.Commons.Core.Domain.Model.Base;
using eTRIKS.Commons.Core.Domain.Model.ControlledTerminology;
using eTRIKS.Commons.Core.Domain.Model.DatasetModel;

namespace eTRIKS.Commons.Core.Domain.Model
{
    public abstract class Characterisitc : Identifiable<int>
    {
        public CharacteristicObject CharacteristicObject { get; set; }
        public int CharacteristicObjectId { get; set; }
        public string VerbatimName { get; set; }//will use to refer to characteristicObject Verbatim name 
        public string VerbatimValue { get; set; }
        public CVterm ControlledValue { get; set; }
        public string ControlledValueStr { get; set; }
        public string CVtermId { get; set; }
        public string DatasetDomainCode { get; set; }
        public VariableReference DatasetVariable { get; set; }
        public int? DatasetVariableId { get; set; }
        //public Dataset Dataset { get; set; }
        public int DatasetId { get; set; }
    }
}

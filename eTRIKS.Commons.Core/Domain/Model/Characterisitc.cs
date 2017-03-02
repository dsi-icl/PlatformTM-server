using eTRIKS.Commons.Core.Domain.Model.Base;
using eTRIKS.Commons.Core.Domain.Model.ControlledTerminology;
using eTRIKS.Commons.Core.Domain.Model.DatasetModel;

namespace eTRIKS.Commons.Core.Domain.Model
{
    public abstract class Characterisitc : Identifiable<int>
    {
        public CharacteristicFeature CharacteristicFeature { get; set; }
        public int CharacteristicFeatureId { get; set; }
        
        public string VerbatimValue { get; set; }
       
        public string ControlledValueStr { get; set; } //temporary
        public string VerbatimName { get; set; }//will use to refer to characteristicObject Verbatim name 

        //public string Datatype { get; set; }

        //public string DatasetDomainCode { get; set; }

        public CVterm ControlledValue { get; set; }
        public string CVtermId { get; set; }

        public DataFile Datafile { get; set; }
        public int? DatafileId { get; set; }

        public Dataset Dataset { get; set; }
        public int DatasetId { get; set; }
    }
}

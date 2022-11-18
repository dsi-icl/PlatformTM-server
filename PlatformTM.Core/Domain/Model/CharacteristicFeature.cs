using PlatformTM.Core.Domain.Model.Base;
using PlatformTM.Core.Domain.Model.ControlledTerminology;
using PlatformTM.Core.Domain.Model.DatasetModel.PDS;

namespace PlatformTM.Core.Domain.Model
{
    public class CharacteristicFeature : Identifiable<int>
    {
        public string ShortName { get; set; }
        public string FullName { get; set; }
        public string DataType { get; set; }
        public string Domain { get; set; }

        public CVterm ControlledTerm { get; set; }
        public string CVtermId { get; set; }
       

        
        public PrimaryDataset Dataset { get; set; }
        public int? DatasetId { get; set; }

        public Project Project { get; set; }
        public int ProjectId { get; set; }
    }
}


using PlatformTM.Core.Domain.Model.DatasetModel;

namespace PlatformTM.Core.JoinEntities
{
    public class DatasetDatafile
    {
        public int DatasetId { get; set; }
        public Dataset Dataset { get; set; }
        public int DatafileId { get; set; }
        public DataFile Datafile { get; set; }
    }
}

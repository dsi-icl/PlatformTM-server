
using eTRIKS.Commons.Core.Domain.Model.DatasetModel;

namespace eTRIKS.Commons.Core.JoinEntities
{
    public class DatasetDatafile
    {
        public int DatasetId { get; set; }
        public Dataset Dataset { get; set; }
        public int DatafileId { get; set; }
        public DataFile Datafile { get; set; }
    }
}

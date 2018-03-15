using PlatformTM.Core.Domain.Model;
using PlatformTM.Core.Domain.Model.DatasetModel;

namespace PlatformTM.Core.JoinEntities
{
    public class StudyDataset
    {
        public int DatasetId { get; set; }
        public Dataset Dataset { get; set; }

        public int StudyId { get; set; }
        public Study Study { get; set; }
    }
}

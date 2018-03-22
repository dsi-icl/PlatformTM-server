using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Core.Domain.Model.DatasetModel;
using eTRIKS.Commons.Core.Domain.Model.DesignElements;

namespace eTRIKS.Commons.Core.JoinEntities
{
    public class StudyDataset
    {
        public int DatasetId { get; set; }
        public Dataset Dataset { get; set; }

        public int StudyId { get; set; }
        public Study Study { get; set; }
    }
}

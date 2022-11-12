using System.Collections.Generic;
using PlatformTM.Core.Domain.Model.DatasetModel.PDS;

namespace PlatformTM.Core.Domain.Model
{
    public class SubjectRecording : Assessment
    {
        public string SubjectType { get; set; }

        public SubjectRecording()
        {
            Datasets = new List<PrimaryDataset>();
        }
    }
}
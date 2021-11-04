using System.Collections.Generic;
using PlatformTM.Core.Domain.Model.Base;
using PlatformTM.Core.Domain.Model.DatasetModel;

namespace PlatformTM.Core.Domain.Model
{
    public class SubjectRecording : Activity
    {
        public string SubjectType { get; set; }

        public SubjectRecording()
        {
            Datasets = new List<Dataset>();
        }
    }
}
using System.Collections.Generic;
using PlatformTM.Core.Domain.Model.Base;
using PlatformTM.Core.Domain.Model.DatasetModel;
using PlatformTM.Core.Domain.Model.DatasetModel.PDS;
using PlatformTM.Core.Domain.Model.DesignElements;

namespace PlatformTM.Core.Domain.Model
{
    public class Assessment : Identifiable<int>
    {

        public string Name { get; set; } 
        public string Description { get; set; }
        public string Status { get; set; }

        public int StudyId { get; set; }
        public Study Study { get; set; }

        public int? TimeEventId { get; set; }
        public Visit TimeEvent { get; set; }

        public IList<PrimaryDataset> Datasets { get; set; }


        public Assessment()
        {
            Datasets = new List<PrimaryDataset>();
            StudyId = 0;
            Name=Description=Status="";
        }
    }
}
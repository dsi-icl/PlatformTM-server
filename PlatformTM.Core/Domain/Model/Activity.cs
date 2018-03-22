using System.Collections.Generic;
using PlatformTM.Core.Domain.Model.Base;
using PlatformTM.Core.Domain.Model.DatasetModel;

namespace PlatformTM.Core.Domain.Model
{
    public class Activity : Identifiable<int>
    {
        //public string OID { get; set; }
        public string Name { get; set; }
        public int ProjectId { get; set; }

        public ICollection<Dataset> Datasets { get; set; }
        //public List<Study> Studies { get; set; }
        public Project Project { get; set; }

        public Activity()
        {
            Datasets = new List<Dataset>();
        }
    }
}
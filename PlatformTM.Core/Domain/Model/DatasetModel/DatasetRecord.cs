using System.Collections.Generic;

namespace PlatformTM.Core.Domain.Model.DatasetModel
{
    public class DatasetRecord : Dictionary<string,string>
    {

        public string DatasetId { get; set; }
        
        public PrimaryDataset Dataset { get; set; }


      
    }
}
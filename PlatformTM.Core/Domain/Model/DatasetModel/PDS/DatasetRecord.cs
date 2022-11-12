using System.Collections.Generic;

namespace PlatformTM.Core.Domain.Model.DatasetModel.PDS
{
    public class DatasetRecord : Dictionary<string,string>
    {


        public string DatasetId { get; set; }
        public PrimaryDataset Dataset { get; set; }

        public int DataFile { get; set; }
        public DataFile SourceDataFile { get; set; }


      
    }
}
using System;
using System.Collections.Generic;
using PlatformTM.Core.Domain.Model.DatasetModel;

namespace PlatformTM.Core.Domain.Model.DatasetDescriptorTypes
{
    //Primary Data Descriptor 
    public class DatasetDescriptor
    {
        public string Title { set; get; }
        public string Description { get; set; }

        //public string Class { get; set; }
        //public string Description { get; set; }
        //public string Code { get; set; }
        //public string Structure { get; set; }
        //public bool IsRepeating { get; set; }

        //public CVterm Domain { get; set; }

        public DatasetType DatasetType { get; set; }
        //public Identifiable<int> DatasetEntity { get; set; }
        //public string DatasetType { get; set; }
        public string DatasetId { get; set; }

        public PrimaryDataset Dataset { get; set; }

  


    }

    public interface IObservationDatasetDescriptor
    {
        DatasetDescriptor GetDatasetDescriptor();
    }

    

    


}

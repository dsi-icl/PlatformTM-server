using System;
using System.Collections.Generic;
using PlatformTM.Core.Domain.Model.Base;
using PlatformTM.Core.Domain.Model.DatasetModel.PDS.DatasetFieldTypes;

namespace PlatformTM.Core.Domain.Model.DatasetModel.PDS.DatasetDescriptorTypes
{
    //Primary Data Descriptor 
    public class DatasetDescriptor : Identifiable<Guid>
    {
        public string Title { set; get; }
        public string Description { get; set; }

        public DatasetType DatasetType { get; set; }


        //public Project project { get; set; }
        public int ProjectId { get; set; }

        //public PrimaryDataset Dataset { get; set; }
        //public int DatasetId { get; set; }


        public List<DatasetField> GetDatasetFields()
        {

            throw new NotImplementedException();
        }

    }

    public interface IObservationDatasetDescriptor
    {
        DatasetDescriptor GetDatasetDescriptor();
    }


    


}

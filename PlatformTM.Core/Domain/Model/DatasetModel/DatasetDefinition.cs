using System;
using System.Collections.Generic;
using PlatformTM.Core.Domain.Model.Base;

namespace PlatformTM.Core.Domain.Model.DatasetModel
{
    public class DatasetDefinition
    {
       // public CVterm Domain { get; set; } 
        
        public DatasetType DatasetType { get; set; }
        public Identifiable<int> DatasetEntity { get; set; }

        public ICollection<DatasetField> Fields { get; private set; }

        public DatasetDefinition()
        {
            Fields = new List<DatasetField>();
        }

        
    }

    public enum DatasetType
    {
         ObservationDatasetType,
         FeatureDatasetType,
         SubjectDatasetType,
         SampleDatasetType,
         StudyDatasetType
    }
}

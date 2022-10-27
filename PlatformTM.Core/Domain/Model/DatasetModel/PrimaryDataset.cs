using System;
using System.Collections.Generic;
using PlatformTM.Core.Domain.Model.DatasetDescriptorTypes;

namespace PlatformTM.Core.Domain.Model.DatasetModel
{
    public class PrimaryDataset
    {

        public string ProjectId { get; set; }
        public string StudyId { get; set; }

        public DatasetDescriptor DatasetDescriptor { get; set; }
        public string DescriptorId { get; set; }

        public List<DatasetRecord> DataRecords { get; set; }
        //Related files in the same dataset OR multiple datasets linked


        public Project Project { get; set; }
        public Study Study { get; set; }
        public PrimaryDataset()
        {
            DataRecords = new List<DatasetRecord>();
        }

        public void GetDatasetFeatures()
        {

        }

        public void GetDatasetObservedPhenomena()
        {

        }

        public DatasetRecord NewRecord()
        {
            var datasetRecord = new DatasetRecord();
            DataRecords.Add(datasetRecord);

            //datasetRecord.Add(DatasetDescriptor.SubjectIdentifierField.Name)

            //datasetRecord[datasetDescriptor.SubjectIdentifierField.Name] = "";

            ////StudyName
            //datasetRecord[datasetDescriptor.StudyIdentifierField.Name] = "";

            ////FeatureCategory
            //datasetRecord[datasetDescriptor.C.Name] = oMapper.GetFeatureName();

            ////FeatureName
            //datasetRecord[datasetDescriptor.FeatureNameField.Name] = oMapper.GetFeatureName();
            return new DatasetRecord();

        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using PlatformTM.Core.Domain.Model.Base;
using PlatformTM.Core.Domain.Model.DatasetModel.PDS.DatasetDescriptorTypes;

namespace PlatformTM.Core.Domain.Model.DatasetModel.PDS
{
    public class PrimaryDataset : Identifiable<int>
    {

        public string Title { get; set; }
        public string Description { get; set; }
        public string Domain { get; set; }

        public string DateCreated { get; set; }
        public string Version { get; set; }
        public string LastUpdated { get; set; }


        public Project Project { get; set; }
        public int ProjectId { get; set; }

        public Study Study { get; set; }
        public int StudyId { get; set; }

 
        public DatasetDescriptor DatasetDescriptor { get; set; }
        public int DescriptorId { get; set; }



        public List<DatasetRecord> DataRecords { get; set; }



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

        public DataTable TabulariseDataset()
        {
            var datasetDT = new DataTable();
            datasetDT.TableName = DatasetDescriptor.Title;

            //Add Fields
            var allFields = ((ObservationDatasetDescriptor)DatasetDescriptor).GetDatasetFields();
            foreach (var field in allFields)
            {
                datasetDT.Columns.Add(field.Name);
            }

            //Add Data
            foreach (var record in DataRecords)
            {

                var row = datasetDT.NewRow();
                foreach (var kv in record)
                {
                    row[kv.Key] = kv.Value;
                }

                datasetDT.Rows.Add(row);
            }
            return datasetDT;

        }
    }
}

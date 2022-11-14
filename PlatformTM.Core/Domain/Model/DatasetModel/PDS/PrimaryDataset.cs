using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using PlatformTM.Core.Domain.Model.Base;
using PlatformTM.Core.Domain.Model.BMO;
using PlatformTM.Core.Domain.Model.DatasetModel.PDS.DatasetDescriptorTypes;

namespace PlatformTM.Core.Domain.Model.DatasetModel.PDS
{
    public class PrimaryDataset : Versionable<int>
    {

        public string Title { get; set; }
        public string Description { get; set; }
        public string Acronym { get; set; }
        public string Domain { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; }

        public Guid DescriptorId { get; set; }
        public DatasetDescriptor Descriptor { get; set; }
        
        public IList<DataFile> DataFiles { get; set; }
        public IList<Assessment> Assessments { get; set; }
        public IList<Study> Studies { get; set; }
        public IList<Feature> ObservedFeatures { get; set; }

        //ignore from DB
        public List<DatasetRecord> DataRecords { get; set; }
        

        public PrimaryDataset()
        {
            DataRecords = new List<DatasetRecord>();
        }

        public void GetDatasetFeatures()
        {
            var dd = (ObservationDatasetDescriptor)Descriptor;


            var uniqueFeatures = DataRecords.Select(r => r[dd.FeatureNameField.Name]).Distinct();

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
            datasetDT.TableName = Descriptor.Title;

            //Add Fields
            var allFields = ((ObservationDatasetDescriptor)Descriptor).GetDatasetFields();
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

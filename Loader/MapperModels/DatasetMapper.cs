using System;
using PlatformTM.Core.Domain.Model.DatasetModel;

namespace PlatformTM.Models
{
    public class DatasetMapper
    {
        public string DatasetName { get; internal set; }
        public string StudyName { get; internal set; }
        public List<ObservationMapper> ObservationMappers { get; internal set; }


        public DatasetMapper()
        {
            ObservationMappers = new List<ObservationMapper>();
        }

        
        public List<string> GetPropertyFields()
        {
            HashSet<string> Properties = new();
            foreach(var obsMapper in ObservationMappers)
            {
                Properties.Add(obsMapper.PropertyMapper.PropertyName);
            }
            return Properties.ToList();
        }

        //public List<string> GetDatasetFeature()
        //{

        //}

        public PrimaryDataset CreatePrimaryDataset()
        {
            PrimaryDataset PrimaryDataset = new() { };
         
            return null;
        }
    }
}


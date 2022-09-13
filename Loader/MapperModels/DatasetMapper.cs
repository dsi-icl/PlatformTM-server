using System;
namespace PlatformTM.Models
{
    public class DatasetMapper
    {
        public DatasetMapper()
        {
            ObservationMappers = new List<ObservationMapper>();
        }

        public string DatasetName { get; internal set; }
        public string StudyName { get; internal set; }
        public List<ObservationMapper> ObservationMappers { get; internal set; }

        public GetObservedFeatures
    }
}


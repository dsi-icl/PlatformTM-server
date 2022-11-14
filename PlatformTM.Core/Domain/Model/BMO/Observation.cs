using System;
using System.Collections.Generic;
using PlatformTM.Core.Domain.Model.Base;
using PlatformTM.Core.Domain.Model.DatasetModel.PDS;

namespace PlatformTM.Core.Domain.Model.BMO
{
    public class Observation:Identifiable<Guid>
    {
        public string SubjectId { get; set; }
        //public HumanSubject ObservedSubject { get; set; }

        public int DatasetId { get; set; }
        //public PrimaryDataset Dataset { get; set; }

        public List<ObservationResult> ObservedPhenomena { get; set; }

        public List<ObservationResult> ObservedFeatureProperties { get; set; }

        public List<ObservationResult> ObservationProperties { get; set; }

        public int FeatureOfInterestId { get; set; }
        //public Observable FeatureOfInterest { get; set; }

        public Observation()
        {
            ObservedPhenomena = new List<ObservationResult>();
            ObservedFeatureProperties = new();
            ObservationProperties = new List<ObservationResult>();
        }
    }
   
}


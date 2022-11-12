using System;
using PlatformTM.Core.Domain.Model.DatasetModel.PDS;

namespace PlatformTM.Core.Domain.Model.BMO
{
    public class Observation
    {
        public HumanSubject ObservedSubject { get; set; }

        public Biosample ObservedSample { get; set; }

        public ObservablePhenomenon ObservedPhenomenon { get; set; }

        public Observable FeatureOfInterest { get; set; }

        public PropertyValue ObservationResult { get; set; }

        public PrimaryDataset Dataset { get; set; }

        public Observation()
        {
        }
    }
}


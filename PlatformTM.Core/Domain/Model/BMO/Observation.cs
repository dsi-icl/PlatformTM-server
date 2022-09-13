﻿using System;
using PlatformTM.Core.Domain.Model.DatasetModel;

namespace PlatformTM.Core.Domain.Model.BMO
{
    public class Observation
    {
        public Subject ObservedSubject { get; set; }

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


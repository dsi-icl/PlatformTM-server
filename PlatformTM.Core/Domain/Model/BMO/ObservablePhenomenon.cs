using System;
using PlatformTM.Core.Domain.Model.Base;
using PlatformTM.Core.Domain.Model.DatasetModel.PDS;

namespace PlatformTM.Core.Domain.Model.BMO
{
    public class ObservablePhenomenon : Identifiable<int>
    {
        public int ObservedFeatureId { get; set; }
        public Feature ObservedFeature { get; set; }

        public int ObservedPropertyId { get; set; }
        public Property ObservedProperty { get; set; }

        public PrimaryDataset Dataset { get; set; }
        public int DatasetId { get; set; }

        public ObservablePhenomenon()
        {
        }
    }

    
}


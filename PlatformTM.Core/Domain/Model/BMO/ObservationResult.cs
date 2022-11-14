using System;
namespace PlatformTM.Core.Domain.Model.BMO
{
    public abstract class ObservationResult
    {
        public int ObservedPhenomenonId { get; set; }
        //public ObservablePhenomenon ObservedPhenomenon { get; set; }

    }

    public class NominalObsResult : ObservationResult
    {
        public string Value { get; set; }
    }

    public class MeasurementObsResult : ObservationResult
    {
        public string Value { get; set; }
        public string Unit { get; set; }
    }

    public class OrdinalObsResult : ObservationResult
    {
        public string Value { get; set; }
        public int Order { get; set; }
    }

}


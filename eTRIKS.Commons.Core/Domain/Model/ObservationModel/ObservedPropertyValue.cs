using System.Runtime.Serialization;
using eTRIKS.Commons.Core.Domain.Model.ControlledTerminology;

namespace eTRIKS.Commons.Core.Domain.Model.ObservationModel
{
    //THIS WOULD BE MAPPED TO ONE TABLE FOR ALL HIERARCHIES
    public  class ObservedPropertyValue
    {
        public PropertyDescriptor Property { get; set; }
        //public int PropertyId { get; set; }
    }

    public class CategoricalValue : ObservedPropertyValue
    {
        public string Value { get; set; }
        public CVterm CVTerm { get; set; }
    }

    public class OrdinalValue : ObservedPropertyValue
    {
        public string Value { get; set; }
        public int Order { get; set; }
    }

    public class NumericalValue : ObservedPropertyValue
    {
        public double Value { get; set; }
        public string Unit { get; set; }
        public CVterm UnitCVterm { get; set; }
    }

    public class IntervalValue : ObservedPropertyValue
    {

    }

    public class MissingValue : ObservedPropertyValue
    {
        public string Value => "";
    }
}

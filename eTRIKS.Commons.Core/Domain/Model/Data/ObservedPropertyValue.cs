using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Model.ControlledTerminology;

namespace eTRIKS.Commons.Core.Domain.Model.Data
{
    public abstract class ObservedPropertyValue
    {
        public PropertyDescriptor Property { get; set; }
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
        public float Value { get; set; }
        public string Unit { get; set; }
        public CVterm UnirCVterm { get; set; }
    }

    public class IntervalValue : ObservedPropertyValue
    {

    }
}

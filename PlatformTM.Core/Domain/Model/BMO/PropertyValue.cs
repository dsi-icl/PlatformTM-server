using System;
namespace PlatformTM.Core.Domain.Model.BMO
{
    public abstract class PropertyValue
    {
        public class CategoricalValue : PropertyValue
        {
            public string Value { get; set; }
            //public CVterm CVTerm { get; set; }
        }

        public class OrdinalValue : PropertyValue
        {
            public string Value { get; set; }
            public int Order { get; set; }
        }

        public class NumericalValue : PropertyValue
        {
            public double Value { get; set; }
            public string Unit { get; set; }
            //public CVterm UnitCVterm { get; set; }
        }

        public PropertyValue()
        {
        }
    }
}


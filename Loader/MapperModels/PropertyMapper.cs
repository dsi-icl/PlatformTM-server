using System;
namespace PlatformTM.Models
{
    public class PropertyMapper
    {
        public string PropertyName { get; set; }
        public string? Unit { get; set; }

        public bool? IsDerived { get; internal set; }
        public bool? IsMultiValued
        {
            get
            {
                return PropertyValueMappers.Count > 1;
            }
        }
        public List<PropertyValueMapper> PropertyValueMappers { get; set; }
        public int Order { get; internal set; }
        public bool IsFeatureProperty { get; internal set; }

        public PropertyMapper(string _name)
        {
            PropertyName = _name;
            PropertyValueMappers = new();
        }

        public bool HasUnit()
        {
            return Unit != null && Unit != "";
        }
    }
}


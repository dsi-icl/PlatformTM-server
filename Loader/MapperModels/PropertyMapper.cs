using System;
namespace PlatformTM.Models
{
    public class PropertyMapper
    {
        public string PropertyName { get; set; }
        public bool IsDerived { get; internal set; }
        public bool IsMultiValued { get; set; }
        public List<PropertyValueMapper> PropertyValueMappers { get; set; }

        public PropertyMapper()
        {
            PropertyValueMappers = new();
        }
    }
}


using System;
namespace PlatformTM.Models
{
    public class ObservationMapper
    {
        public ObservationMapper()
        {
            FeatureNameExpression = new();
            PropertyMappers = new();
        }

        public ObservationMapper(string featureNameExpString)
        {
            FeatureNameExpression = new(featureNameExpString);
            PropertyMappers = new();
        }

        public string? SourceFileName { get; internal set; }
        public string? SourceVariableId { get; internal set; }
        //public string? SourceVariableName { get; internal set; }
        //public string? SourceVariableText { get; internal set; }

        public string? Category { get; internal set; }
        public string? SubCategory { get; internal set; }

        public string? ObsGroupId { get; internal set; }
        public bool IsDerived { get; internal set; }

        public ValueExpression FeatureNameExpression { get; internal set; }
        public List<PropertyMapper> PropertyMappers { get; internal set; }



        public PropertyMapper GetPropertyMapper(string propertyName)
        {
            if (propertyName == null) return null;
            PropertyMapper propMapper = PropertyMappers.Find(p => p.PropertyName.Equals(propertyName));
            return propMapper;
        }

        public PropertyMapper? GetFeaturePropertyMapper()
        {
            return PropertyMappers.Find(p => p.IsFeatureProperty);
        }
    }
}


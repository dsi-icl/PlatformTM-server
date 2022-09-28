using System;
namespace PlatformTM.Models
{
    public class ObservationMapper
    {
        public ObservationMapper()
        {
            ObsFeatureValue = new();
            PropertyMapper = new();
        }

        public string SourceFileName { get; internal set; }
        public string SourceVariableId { get; internal set; }
        public string SourceVariableName { get; internal set; }
        public string SourceVariableText { get; internal set; }
        public string Category { get; internal set; }
        public string ObsGroupId { get; internal set; }
        public bool IsDerived { get; internal set; }

        public ValueExpression ObsFeatureValue { get; internal set; }
        public PropertyMapper PropertyMapper { get; internal set; }



        internal string GetFeatureName()
        {
            throw new NotImplementedException();
        }
    }
}


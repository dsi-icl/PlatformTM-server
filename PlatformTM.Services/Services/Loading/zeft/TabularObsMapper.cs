using System;
using System.Collections.Generic;

namespace PlatformTM.Models.Services.Loading.zeft
{
    public class TabularObsMapper
    {
        public string SourceFileName { get; set; }
        public string SourceVariableId { get; set; }
        public string SourceVariableName { get; set; }
        public string SourceVariableText { get; set; }

        public string StudyName { get; set; }
        public string MappedToEntity { get; set; }
        public string DatasetName { get; set; }

        public string ObservationCategory { get; set; }
        public string ObservationSubcategory { get; set; }
        public string ObservedFeature { get; set; }
        public string ObservationGroupId { get; set; }

        public List<TabularPropMapper> MappedProperties { get; set; }
        public bool IsDerived { get; internal set; }

        public class TabularPropMapper
        {
            public string PropertyName { get; set; }
            public string PropertyValue { get; set; }
            public string PropertyValueUnit { get; set; }

        }

        public TabularObsMapper()
        {
            MappedProperties = new List<TabularPropMapper>();
        }
    }
}


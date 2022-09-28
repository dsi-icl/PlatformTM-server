using System;
using Loader.MapperModels.TabularMapperModels;

namespace PlatformTM.MapperModels.TabularMapperModels
{
    public class TabularEntityMapper
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

        //This stores multiple property Maps that might be mapped in the same mapping record
        public List<TabularPropertyMapper> ObservedPropertyMappers { get; set; }
        public bool IsDerived { get; internal set; }
        public bool IsSkipped { get; internal set; }

        //public class TabularPropMapper
        //{
        //    public string? PropertyName { get; set; }
        //    public string? PropertyValue { get; set; }
        //    public string? PropertyValueUnit { get; set; }

        //    public TabularPropMapper() { }

        //}

        public TabularEntityMapper()
        {
            ObservedPropertyMappers = new List<TabularPropertyMapper>();
        }
    }
}


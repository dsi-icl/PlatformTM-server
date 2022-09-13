using System;
using PlatformTM.MapperModels.TabularMapperModels;

namespace Loader.MapperModels.TabularMapperModels
{
    public class TabularMapper
    {
        public List<TabularEntityMapper> ObsMappers { get; set; }
        public TabularEntityMapper SubjectIdMapper { get; set; }
        public TabularEntityMapper StudyVisitMappper { get; set; }
        public TabularEntityMapper StudyDateOfVisitMapper { get; set; }
        public TabularMapper()
        {
        }
    }
}


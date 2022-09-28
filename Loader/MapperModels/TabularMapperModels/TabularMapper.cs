using System;
using PlatformTM.MapperModels.TabularMapperModels;

namespace Loader.MapperModels.TabularMapperModels
{
    public class TabularMapper
    {
        public List<TabularEntityMapper> EntityMappers { get; set; }

        public TabularEntityMapper SubjectIdMapper {
            get { return EntityMappers.FirstOrDefault(d => d.MappedToEntity == "SUBJID"); }
            
        }
        public TabularEntityMapper? StudyVisitMappper { get; set; }
        public TabularEntityMapper? StudyDateOfVisitMapper { get; set; }
        public TabularMapper()
        {
            EntityMappers = new List<TabularEntityMapper>();
        }


    }
}


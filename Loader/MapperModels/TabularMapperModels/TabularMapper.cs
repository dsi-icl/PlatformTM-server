using System;
using PlatformTM.MapperModels.TabularMapperModels;

namespace Loader.MapperModels.TabularMapperModels
{
    public class TabularMapper
    {
        public List<TabularEntityMapper> EntityMappers { get; set; }

        public TabularEntityMapper? SubjectIdMapper {
            get { return EntityMappers.FirstOrDefault(d => d.MappedToEntity == "$SUBJECTID"); }
            
        }
        public TabularEntityMapper? StudyVisitMappper {
            get { return EntityMappers.FirstOrDefault(d => d.MappedToEntity == "$VISIT"); }
        }
        public TabularEntityMapper? StudyDateOfVisitMapper {
            get { return EntityMappers.FirstOrDefault(d => d.MappedToEntity == "$VISITDATE"); }
        }
        public TabularMapper()
        {
            EntityMappers = new List<TabularEntityMapper>();
        }

        //public List<TabularEntityMapper> RemoveSkippedVariables()
        //{
        //    var skippedRemoved = new List<TabularEntityMapper>() { EntityMappers };
        //    return EntityMappers.RemoveAll(r=>r.IsSkipped)
        //}

        public Dictionary<string,List<TabularEntityMapper>> GroupByDataset()
        {

            EntityMappers.RemoveAll(e => e.IsSkipped);

            var groupedByDataset = EntityMappers.GroupBy(o => o.DatasetName)
                                                .ToDictionary(g => g.Key, g => g.ToList());
            return groupedByDataset;
        }

        public Dictionary<string, List<TabularEntityMapper>> GroupByObsFeature(List<TabularEntityMapper> entityMappers)
        {
            

            var groupedByObsFeature = entityMappers.GroupBy
                (o =>  new {
                    cat=o.ObservationCategory,
                    scat=o.ObservationSubcategory,
                    feat=o.ObservedFeature ,
                    grp=o.ObservationGroupId})
                .ToDictionary(g =>g.Key.ToString() ?? "", g=>g.ToList());

            return groupedByObsFeature;
        }

        internal Object GroupByObservedProperty(List<TabularEntityMapper> entityMappers)
        {
            var groupedByObsProperty = entityMappers.GroupBy
                (o => new {
                    cat = o.PropertyMappers
                })
                .ToDictionary(g => g.Key, g => g.ToList());

            return groupedByObsProperty;
        }

        //This will group multiple rows that belong to the same obsFeature either through repition of the
        //same property like wheeze excaerbating factor, or through different 

        //first thing is to get the properties for each obs feature, then for each property the different value properties
        //
        //CREATE AN OBSERVATION_MAPPER INSTANCE FOR EACH ROW OR GROUP OF ROWS REPRESENTING A SINGLE OBSERVED_FEAURE INSTANCE (Observation)


    }
}


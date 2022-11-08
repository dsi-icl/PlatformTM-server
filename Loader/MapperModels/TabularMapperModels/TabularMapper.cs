using System;
using PlatformTM.MapperModels.TabularMapperModels;

namespace Loader.MapperModels.TabularMapperModels
{
    public class TabularMapper
    {
        public List<TabularEntityMapper> EntityMappers { get; set; }

        public TabularEntityMapper? SubjectIdMapper => EntityMappers.FirstOrDefault(d => d.MappedToEntity == "$SUBJECTID");

        public TabularMapper()
        {
            EntityMappers = new List<TabularEntityMapper>();
        }


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

        public List<TabularEntityMapper> GetNonObsEntityMappers()
        {
            var propertyMappers = new List<TabularEntityMapper>();
            if (EntityMappers.Exists(d => d.MappedToEntity.ToUpper() == "$VISIT"))
                propertyMappers.Add(EntityMappers.First(d => d.MappedToEntity == "$VISIT"));

            if (EntityMappers.Exists(d => d.MappedToEntity.ToUpper() == "$VISITDATE"))
                propertyMappers.Add(EntityMappers.First(d => d.MappedToEntity == "$VISITDATE"));

            if (EntityMappers.Exists(d => d.MappedToEntity.ToUpper() == "$EPOCH"))
                propertyMappers.Add(EntityMappers.First(d => d.MappedToEntity == "$EPOCH"));

            if (EntityMappers.Exists(d => d.MappedToEntity.ToUpper() == "$SRC"))
                propertyMappers.Add(EntityMappers.First(d => d.MappedToEntity == "$SRC"));

            return propertyMappers;
        }

        public string? GetSubjectVariableName()
        {
            return SubjectIdMapper?.SourceVariableName;
        }
        

        //This will group multiple rows that belong to the same obsFeature either through repition of the
        //same property like wheeze excaerbating factor, or through different 

        //first thing is to get the properties for each obs feature, then for each property the different value properties
        //
        //CREATE AN OBSERVATION_MAPPER INSTANCE FOR EACH ROW OR GROUP OF ROWS REPRESENTING A SINGLE OBSERVED_FEAURE INSTANCE (Observation)


    }
}


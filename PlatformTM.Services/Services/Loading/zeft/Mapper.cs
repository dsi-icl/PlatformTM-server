using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using CsvHelper;

namespace PlatformTM.Models.Services.Loading.zeft
{
    public class Mapper
    {
        public Mapper()
        {
        }

        public static List<TabularObsMapper> ReadMappingFile(string MapperFilePath)
        {

            var records = new List<TabularObsMapper>();
            using (var reader = new StreamReader(MapperFilePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {

                csv.Read();
                csv.ReadHeader();
                while (csv.Read())
                {
                    var record = new TabularObsMapper
                    {
                        StudyName = csv.GetField<string>("STUDY_NAME"),
                        SourceFileName = csv.GetField<string>("SOURCE_FILE"),
                        SourceVariableId = csv.GetField<string>("SOURCE_VARIABLE_ID"),
                        SourceVariableName = csv.GetField<string>("SOURCE_VARIABLE_NAME"),
                        SourceVariableText = csv.GetField<string>("SOURCE_VARIABLE_TEXT"),
                        MappedToEntity = csv.GetField<string>("MAP_TO"),
                        DatasetName = csv.GetField<string>("DATASET"),
                        ObservationCategory = csv.GetField<string>("OBS_CAT"),
                        ObservationSubcategory = csv.GetField<string>("OBS_SUBCAT"),
                        ObservedFeature = csv.GetField<string>("OBS_FEATURE"),
                        ObservationGroupId = csv.GetField<string>("OBS_GRP_ID"),
                        IsDerived = csv.GetField<string>("MAP_TO").ToUpper().Equals("$DERIVED"),

                    };

                    Regex rgx = new("(OBS_PROPERTY_)\\d{1}\\Z");

                    foreach (var col in csv.HeaderRecord)
                        if (rgx.IsMatch(col.ToString()))
                        {
                            if (csv[col].ToString() == "") continue;
                            record.MappedProperties.Add(new TabularObsMapper.TabularPropMapper()
                            {
                                PropertyName = csv.GetField<string>(col),
                                PropertyValue = csv.GetField<string>(col + "_VAL"),
                                PropertyValueUnit = csv.GetField<string>(col + "_VAL_UNIT")
                            });

                        }
                    records.Add(record);
                }
            }
            return records;
        }

        //public void ProcessTabularMapper(List<TabularObsMapper> varMappers)
        //{
        //    Dictionary<string, DatasetMapper> DatasetMapper_map = new Dictionary<string, DatasetMapper>();
        //    foreach (var varMapper in varMappers)
        //    {
        //        if (!DatasetMapper_map.TryGetValue(varMapper.DatasetName, out DatasetMapper DatasetMapper))
        //        {
        //            DatasetMapper = new DatasetMapper();
        //        }
        //        DatasetMapper.DatasetName = varMapper.DatasetName;
        //        DatasetMapper.StudyName = varMapper.StudyName;

                

        //        foreach(var prop in varMapper.MappedProperties)
        //        {
        //            var obsMapper = new ObservationMapper()
        //            {
        //                SourceFileName = varMapper.SourceFileName,
        //                SourceVariableId = varMapper.SourceVariableId,
        //                SourceVariableName = varMapper.SourceVariableName,
        //                SourceVariableText = varMapper.SourceVariableText,
                        
        //            };
        //            DatasetMapper.ObservationMappers.Add(obsMapper);

        //            obsMapper.ObsFeatureValue.ValueString = varMapper.ObservedFeature;
        //            obsMapper.Category = varMapper.ObservationCategory;
        //            obsMapper.ObsGroupId = varMapper.ObservationGroupId;

        //            obsMapper.PropertyMapper.PropertyName = prop.PropertyName;
        //            obsMapper.IsDerived = varMapper.IsDerived;

        //            obsMapper.PropertyMapper.PropertyValueMappers.Add(new PropertyValueMapper()
        //            {
        //                ValueString = prop.PropertyValue,
        //                Unit = prop.PropertyValueUnit
        //            });
        //        }
                

        //    }
        //}
    }
}


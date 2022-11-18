using System.Collections.Generic;
using System.Linq;

namespace PlatformTM.Core.Domain.Model.DatasetModel.SDTM
{
    public class SdtmSampleDescriptor : SdtmRowDescriptor
    {
        //public VariableDefinition CharacteristicNameVariable { get; set; }
        //public VariableDefinition CharacteristicShortNameVariable { get; set; }
        
        //public VariableDefinition CharacteristicOriValueVariable { get; set; }
        //public VariableDefinition CharacteristicNumValueVariable { get; set; }
        //public VariableDefinition CharacteristicCharValueVariable { get; set; }
        //public VariableDefinition CharacteristicOriUnitVariable { get; set; }
        //public VariableDefinition CharacteristicStandardUnitVariable { get; set; }
        //public VariableDefinition SpecimenTypeVariable { get; set; }
        //public VariableDefinition AnatomicRegionVariable { get; set; }

        //public VariableDefinition IsBaseLineVariable { get; set; }

        ////const string NumericResVarName = "BSSTRESN";
        ////const string CharResVarName = "BSSTRESC";
        ////const string OriResVarName = "BSORRES";
        ////const string OriResUnitVarName = "BSORRESU";
        ////const string StdResUnitVarName = "BSSTRESU";

        //public static SdtmSampleDescriptor GetSdtmSampleDescriptor(Dataset dataset)
        //{
        //    var descriptor = new SdtmSampleDescriptor();

        //    descriptor.Class = dataset.Template.Class;
        //    descriptor.Domain = dataset.Template.Domain;
        //    descriptor.DomainCode = dataset.Template.Code;



        //    //IDENTIFIERS
        //    descriptor.StudyIdentifierVariable =
        //        dataset.Variables.Single(v => v.VariableDefinition.Name == "STUDYID").VariableDefinition;
        //    descriptor.DomainVariable =
        //       dataset.Variables.Single(v => v.VariableDefinition.Name == "DOMAIN").VariableDefinition;
        //    descriptor.UniqueSubjIdVariable =
        //       dataset.Variables.Single(v => v.VariableDefinition.Name == "USUBJID").VariableDefinition;
        //    descriptor.SampleIdVariable =
        //       dataset.Variables.SingleOrDefault(v => v.VariableDefinition.Name.EndsWith("REFID"))?.VariableDefinition;

        //    //GROUP DESCRIPTORS
        //    descriptor.GroupVariable = dataset.Variables
        //                .Select(l => l.VariableDefinition).FirstOrDefault(v => v.Name == descriptor.DomainCode + "CAT");
        //    descriptor.SubgroupVariable = dataset.Variables
        //               .Select(l => l.VariableDefinition).FirstOrDefault(v => v.Name == descriptor.DomainCode + "SCAT");


        //    //CharacteristicName
        //    descriptor.CharacteristicShortNameVariable = dataset.Variables
        //        .Select(l => l.VariableDefinition).FirstOrDefault(v => v.Name == "BSTESTCD");
        //    descriptor.CharacteristicNameVariable = dataset.Variables
        //        .Select(l => l.VariableDefinition).FirstOrDefault(v => v.Name == "BSTEST");
            

        //    //CharacteristicValue
        //    descriptor.CharacteristicOriValueVariable = dataset.Variables
        //        .Select(l => l.VariableDefinition).FirstOrDefault(v => v.Name == "BSORRES");
        //    descriptor.CharacteristicNumValueVariable = dataset.Variables
        //        .Select(l => l.VariableDefinition).FirstOrDefault(v => v.Name == "BSSTRESN");
        //    descriptor.CharacteristicCharValueVariable = dataset.Variables
        //        .Select(l => l.VariableDefinition).FirstOrDefault(v => v.Name == "BSSTRESC");

        //    //CharacteristicValue unit
        //    descriptor.CharacteristicOriUnitVariable = dataset.Variables
        //        .Select(l => l.VariableDefinition).FirstOrDefault(v => v.Name == "BSORRESU");
        //    descriptor.CharacteristicStandardUnitVariable = dataset.Variables
        //        .Select(l => l.VariableDefinition).FirstOrDefault(v => v.Name == "BSSTRESU");

        //    //VISIT
        //    descriptor.VisitNameVariable =
        //        dataset.Variables.SingleOrDefault(v => v.VariableDefinition.Name == "VISIT")?.VariableDefinition;
        //    descriptor.VisitNumVariable =
        //        dataset.Variables.SingleOrDefault(v => v.VariableDefinition.Name == "VISITNUM")?.VariableDefinition;
        //    descriptor.VisitPlannedStudyDay =
        //        dataset.Variables.SingleOrDefault(v => v.VariableDefinition.Name == "VISITDY")?.VariableDefinition;

        //    //DATETIME
        //    descriptor.DateTimeVariable =
        //        dataset.Variables.SingleOrDefault(v => v.VariableDefinition.Name == descriptor.DomainCode + "DTC")?.VariableDefinition;

        //    //STUDYDAY
        //    descriptor.StudyDayVariable =
        //       dataset.Variables.SingleOrDefault(v => v.VariableDefinition.Name == descriptor.DomainCode + "DY")?.VariableDefinition;

        //    //TIMEPOINT
        //    descriptor.TimePointNameVariable =
        //        dataset.Variables.SingleOrDefault(v => v.VariableDefinition.Name == descriptor.DomainCode + "TPT")?.VariableDefinition;
        //    descriptor.TimePointNumberVariable =
        //       dataset.Variables.SingleOrDefault(v => v.VariableDefinition.Name == descriptor.DomainCode + "TPTNUM")?.VariableDefinition;

        //    descriptor.IsBaseLineVariable =
        //        dataset.Variables.SingleOrDefault(v => v.VariableDefinition.Name == descriptor.DomainCode + "BLFL")?
        //            .VariableDefinition;
        //    return descriptor;
        //}

        //public VariableDefinition GetValueVariable(SdtmRow sdtmRow)
        //{
        //    if (CharacteristicNumValueVariable != null && sdtmRow.ResultQualifiers.ContainsKey(CharacteristicNumValueVariable.Name) && sdtmRow.ResultQualifiers[CharacteristicNumValueVariable.Name] != "")
        //    {
        //        return CharacteristicNumValueVariable;
        //    }
        //    if (CharacteristicCharValueVariable !=null && sdtmRow.ResultQualifiers.ContainsKey(CharacteristicCharValueVariable.Name) && sdtmRow.ResultQualifiers[CharacteristicCharValueVariable.Name] != "")
        //    {
        //        return CharacteristicCharValueVariable;
        //    }
        //    return CharacteristicOriValueVariable;
        //}

        //public VariableDefinition GetUnitVariable(SdtmRow sdtmRow)
        //{
        //    if (sdtmRow.QualifierQualifiers.ContainsKey(CharacteristicStandardUnitVariable.Name))
        //        return CharacteristicStandardUnitVariable;
        //    if(sdtmRow.QualifierQualifiers.ContainsKey(CharacteristicOriUnitVariable.Name))
        //        return CharacteristicOriUnitVariable;
        //    return null;
        //}

        //public List<VariableDefinition> GetTimingVariables()
        //{
        //    return new List<VariableDefinition>() {DateTimeVariable,StudyDayVariable};
        //}
    }
}

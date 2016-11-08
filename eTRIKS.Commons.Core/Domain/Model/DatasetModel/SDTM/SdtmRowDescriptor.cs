

using System.Collections.Generic;
using System.Linq;

namespace eTRIKS.Commons.Core.Domain.Model.Data.SDTM
{
    public class SdtmRowDescriptor
    {
        public string Class { get; set; }
        public string Domain { get; set; }
        public string DomainCode { get; set; }

        public VariableDefinition StudyIdentifierVariable { get; set; }
        public VariableDefinition DomainVariable { get; set; }
        public VariableDefinition UniqueSubjIdVariable { get; set; }
        public VariableDefinition SubjIdVariable { get; set; }
        public VariableDefinition SampleIdVariable { get; set; }


        public VariableDefinition TopicVariable { get; set; } //VSTESTCD // AETERM // CMTRT
        public VariableDefinition TopicSynonymVariable { get; set; } //--TEST //--MODIFY
        public VariableDefinition TopicCVtermVariable { get; set; } //--LOINC // --DECOD

        public VariableDefinition GroupVariable { get; set; } //--CAT
        public VariableDefinition SubgroupVariable { get; set; } //--SCAT

        public List<VariableDefinition> QualifierVariables { get; set; } //same catergory as DefaultQualifier ... still not sure about the name
        public List<VariableDefinition> ResultVariables { get; set; }
        public List<VariableDefinition> SynonymVariables { get; set; }
        public List<VariableDefinition> VariableQualifierVariables { get; set; }
        public VariableDefinition DefaultQualifier { get; set; }

        //VISIT VARIABLES
        public VariableDefinition VisitNameVariable { get; set; }
        public VariableDefinition VisitNumVariable { get; set; }
        public VariableDefinition VisitPlannedStudyDay { get; set; }

        //TIME VARIABLES
        public VariableDefinition DateTimeVariable { get; set; } //--DTC
        public VariableDefinition StudyDayVariable { get; set; } //--DY
        public VariableDefinition TimePointNameVariable { get; set; } //--TPT
        public VariableDefinition TimePointNumberVariable { get; set; } //--TPTNUM
        public VariableDefinition DurationVariable { get; set; } //--DUR

        public VariableDefinition StartDateTimeVariable { get; set; } //--STDTC
        public VariableDefinition EndDateTimeVariable { get; set; } //--ENDTC

        public VariableDefinition StartStudyDayVariable { get; set; } //--STDY
        public VariableDefinition EndStudyDayVariable { get; set; } //--ENDY






        public Dictionary<string, VariableDefinition> name2variable { get; set; }

        public static SdtmRowDescriptor GetSdtmRowDescriptor(Dataset dataset)
        {
            var descriptor = new SdtmRowDescriptor();

            descriptor.Class = dataset.Domain.Class;
            descriptor.Domain = dataset.Domain.Name;
            descriptor.DomainCode = dataset.Domain.Code;

            //IDENTIFIERS
            descriptor.StudyIdentifierVariable =
                dataset.Variables.Single(v => v.VariableDefinition.Name == "STUDYID").VariableDefinition;
            descriptor.DomainVariable =
               dataset.Variables.Single(v => v.VariableDefinition.Name == "DOMAIN").VariableDefinition;
            descriptor.SubjIdVariable =
                dataset.Variables.SingleOrDefault(v => v.VariableDefinition.Name == "SUBJID")?.VariableDefinition;
            descriptor.UniqueSubjIdVariable =
               dataset.Variables.Single(v => v.VariableDefinition.Name == "USUBJID").VariableDefinition;
            descriptor.SampleIdVariable =
               dataset.Variables.SingleOrDefault(v => v.VariableDefinition.Name.EndsWith("REFID"))?.VariableDefinition;

            //TOPIC DESCRIPTORS
            descriptor.TopicVariable = dataset.Variables
                        .Select(l => l.VariableDefinition).FirstOrDefault(v => v.RoleId == "CL-Role-T-2");

            descriptor.TopicCVtermVariable = dataset.Variables
                        .Select(l => l.VariableDefinition).FirstOrDefault(v => v.Name == descriptor.DomainCode + "DECOD");
            if (dataset.Domain.Class.ToLower().Equals("findings"))
                descriptor.TopicCVtermVariable = dataset.Variables
                    .Select(l => l.VariableDefinition).FirstOrDefault(v => v.Name == descriptor.DomainCode + "LOINC");

            descriptor.TopicSynonymVariable = dataset.Variables
                    .Select(l => l.VariableDefinition).FirstOrDefault(v => v.Name == descriptor.DomainCode + "MODIFY");
            if (dataset.Domain.Class.ToLower().Equals("findings") || dataset.Domain.Code.ToLower().Equals("bs"))
                descriptor.TopicSynonymVariable = dataset.Variables
                    .Select(l => l.VariableDefinition).FirstOrDefault(v => v.Name == descriptor.DomainCode + "TEST");


            //GROUP DESCRIPTORS
            descriptor.GroupVariable = dataset.Variables
                        .Select(l => l.VariableDefinition).FirstOrDefault(v => v.Name == descriptor.DomainCode + "CAT");
            descriptor.SubgroupVariable = dataset.Variables
                       .Select(l => l.VariableDefinition).FirstOrDefault(v => v.Name == descriptor.DomainCode + "SCAT");


            //QUALIFIERS
            descriptor.QualifierVariables = dataset.Variables
                        .Select(l => l.VariableDefinition)
                        .Where(v => v.RoleId == "CL-Role-T-3")
                        .ToList();
            descriptor.SynonymVariables = dataset.Variables
                        .Select(l => l.VariableDefinition)
                        .Where(v => v.RoleId == "CL-Role-T-4")
                        .ToList();
            descriptor.VariableQualifierVariables = dataset.Variables
                        .Select(l => l.VariableDefinition)
                        .Where(v => v.RoleId == "CL-Role-T-5")
                        .ToList();
            descriptor.ResultVariables = dataset.Variables
                        .Select(l => l.VariableDefinition)
                        .Where(v => v.RoleId == "CL-Role-T-8")
                        .ToList();

            //VISIT
            descriptor.VisitNameVariable =
                dataset.Variables.SingleOrDefault(v => v.VariableDefinition.Name == "VISIT")?.VariableDefinition;
            descriptor.VisitNumVariable =
                dataset.Variables.SingleOrDefault(v => v.VariableDefinition.Name == "VISITNUM")?.VariableDefinition;
            descriptor.VisitPlannedStudyDay =
                dataset.Variables.SingleOrDefault(v => v.VariableDefinition.Name == "VISITDY")?.VariableDefinition;

            //DATETIME
            descriptor.DateTimeVariable=
                dataset.Variables.SingleOrDefault(v => v.VariableDefinition.Name == descriptor.DomainCode + "DTC")?.VariableDefinition;
            
            //STUDYDAY
            descriptor.StudyDayVariable =
               dataset.Variables.SingleOrDefault(v => v.VariableDefinition.Name == descriptor.DomainCode + "DY")?.VariableDefinition;

            //TIMEPOINT
            descriptor.TimePointNameVariable =
                dataset.Variables.SingleOrDefault(v => v.VariableDefinition.Name == descriptor.DomainCode + "TPT")?.VariableDefinition;
            descriptor.TimePointNumberVariable =
               dataset.Variables.SingleOrDefault(v => v.VariableDefinition.Name == descriptor.DomainCode + "TPTNUM")?.VariableDefinition;

            //DURATION (Collected NOT derived)
            descriptor.DurationVariable =
                dataset.Variables.SingleOrDefault(v => v.VariableDefinition.Name == descriptor.DomainCode + "DUR")?.VariableDefinition;

            //START DATETIME
            descriptor.StartDateTimeVariable =
                dataset.Variables.SingleOrDefault(v => v.VariableDefinition.Name == descriptor.DomainCode + "STDTC")?.VariableDefinition;
            //END DATETIME
            descriptor.EndDateTimeVariable =
               dataset.Variables.SingleOrDefault(v => v.VariableDefinition.Name == descriptor.DomainCode + "ENDTC")?.VariableDefinition;
            //START STUDY DAY
            descriptor.StartStudyDayVariable =
               dataset.Variables.SingleOrDefault(v => v.VariableDefinition.Name == descriptor.DomainCode + "STDY")?.VariableDefinition;
            //END STUDY DAY
            descriptor.EndStudyDayVariable =
               dataset.Variables.SingleOrDefault(v => v.VariableDefinition.Name == descriptor.DomainCode + "ENDY")?.VariableDefinition;
            
            
            //RFTDTC
            return descriptor;
        }
    }


}

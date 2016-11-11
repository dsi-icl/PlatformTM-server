using System.Collections.Generic;
using System.Linq;

namespace eTRIKS.Commons.Core.Domain.Model.DatasetModel.SDTM
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

        //SUBJECT SPECIFIC VARIABLES
        public VariableDefinition ArmVariable { get; set; }
        public VariableDefinition ArmCodeVariable { get; set; }
        public VariableDefinition RefStartDate { get; set; }
        public VariableDefinition RefEndDate { get; set; }
        public VariableDefinition SiteIdVariable { get; set; }

        public bool ObsIsAFinding { get; set; } = false;
        public bool ObsIsAnEvent { get; set; } = false;
        public bool ObsIsMedDRAcoded { get; set; }
        public SdtmMedDRADescriptors MedDRAvariables { get; set; }






        public Dictionary<string, VariableDefinition> name2variable { get; set; }

        public static SdtmRowDescriptor GetSdtmRowDescriptor(Dataset dataset)
        {
            var descriptor = new SdtmRowDescriptor();

            descriptor.Class = dataset.Domain.Class;
            descriptor.Domain = dataset.Domain.Name;
            descriptor.DomainCode = dataset.Domain.Code;

            descriptor.ObsIsAFinding = descriptor.Class.ToUpper() == ("FINDINGS");
            descriptor.ObsIsAnEvent = descriptor.Class.ToUpper() == "EVENTS";

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

            //MedDRAVariables
            if (descriptor.ObsIsAnEvent)
            {
                descriptor.MedDRAvariables = SdtmMedDRADescriptors.GetSdtmMedDRADescriptors(dataset);
                // descriptor.ObsIsMedDRAcoded = SdtmMedDRADescriptors.
            }

            //VISIT
            descriptor.VisitNameVariable =
                dataset.Variables.SingleOrDefault(v => v.VariableDefinition.Name == "VISIT")?.VariableDefinition;
            descriptor.VisitNumVariable =
                dataset.Variables.SingleOrDefault(v => v.VariableDefinition.Name == "VISITNUM")?.VariableDefinition;
            descriptor.VisitPlannedStudyDay =
                dataset.Variables.SingleOrDefault(v => v.VariableDefinition.Name == "VISITDY")?.VariableDefinition;

            //DATETIME
            descriptor.DateTimeVariable =
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


            //DEMOGRAPHICS SPECIFIC VARIABLES
            //ARM
            descriptor.ArmVariable = dataset.Variables.SingleOrDefault(v => v.VariableDefinition.Name == "ARM")?.VariableDefinition;
            //ARMCODE
            descriptor.ArmCodeVariable = dataset.Variables.SingleOrDefault(v => v.VariableDefinition.Name == "ARMCD")?.VariableDefinition;
            //Subject Reference Start Date
            descriptor.RefStartDate = dataset.Variables.SingleOrDefault(v => v.VariableDefinition.Name == "RFSTDTC")?.VariableDefinition;
            //Reference End Date
            descriptor.RefEndDate = dataset.Variables.SingleOrDefault(v => v.VariableDefinition.Name == "RFENDTC")?.VariableDefinition;
            //SITE ID
            descriptor.SiteIdVariable = dataset.Variables.SingleOrDefault(v => v.VariableDefinition.Name == "SITEID")?.VariableDefinition;

            return descriptor;
        }

        public VariableDefinition GetDefaultQualifier(SdtmRow sdtmRow)
        {
            const string numResVar = "STRESN";
            const string charResVar = "STRESC";
            const string oriResVar = "ORRES";
            const string occurVar = "SEV";
            string s;
            if (ObsIsAFinding)
            {
                if (sdtmRow.ResultQualifiers.TryGetValue(DomainCode+numResVar, out s) && sdtmRow.ResultQualifiers[DomainCode + numResVar] != "")
                {
                    return ResultVariables.Find(rv => rv.Name.Equals(DomainCode + numResVar));
                }
                else if (sdtmRow.ResultQualifiers.TryGetValue(DomainCode+charResVar, out s) && sdtmRow.ResultQualifiers[DomainCode + charResVar] != "")
                {
                    return ResultVariables.Find(rv => rv.Name.Equals(DomainCode + charResVar));
                }
                else
                {
                    return ResultVariables.Find(rv => rv.Name.Equals(DomainCode + oriResVar));
                }
            }
            if (ObsIsAnEvent)
            {
                return QualifierVariables.Find(rv => rv.Name.Equals(DomainCode + occurVar));
            }
            return null;
        }
    }

}

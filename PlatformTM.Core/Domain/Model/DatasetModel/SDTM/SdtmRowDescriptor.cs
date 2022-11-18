using System.Collections.Generic;
using System.Linq;
using PlatformTM.Core.Domain.Model.Templates;

namespace PlatformTM.Core.Domain.Model.DatasetModel.SDTM
{
    public class SdtmRowDescriptor
    {
        public string Class { get; set; }
        public string Domain { get; set; }
        public string DomainCode { get; set; }

        public DatasetTemplateField StudyIdentifierVariable { get; set; }
        public DatasetTemplateField DomainVariable { get; set; }
        public DatasetTemplateField UniqueSubjIdVariable { get; set; }
        public DatasetTemplateField SubjIdVariable { get; set; }
        public DatasetTemplateField SampleIdVariable { get; set; }


        public DatasetTemplateField TopicVariable { get; set; } //VSTESTCD // AETERM // CMTRT
        public DatasetTemplateField TopicSynonymVariable { get; set; } //--TEST //--MODIFY
        public DatasetTemplateField TopicCVtermVariable { get; set; } //--LOINC // --DECOD

        //public ObservationModel.NumericMeasureQF OriginalMeasurmentResult;
        //public ObservationModel.NumericMeasureQF StandardMeasurmentResult;
        //public ObservationModel.CodedQualifiedField Topic;


        //List<DatasetTemplateField> FeatureProperties;


        public DatasetTemplateField GroupVariable { get; set; } //--CAT
        public DatasetTemplateField SubgroupVariable { get; set; } //--SCAT

        public List<DatasetTemplateField> QualifierFields { get; set; } //same catergory as DefaultQualifier ... still not sure about the name
        public List<DatasetTemplateField> ResultFields { get; set; }
        public List<DatasetTemplateField> SynonymFields { get; set; }
        public List<DatasetTemplateField> VariableQualifierFields { get; set; }
        public List<DatasetTemplateField> TimeFields { get; set; }
        public DatasetTemplateField DefaultQualifier { get; set; }

        //VISIT Fields
        public DatasetTemplateField VisitNameVariable { get; set; }
        public DatasetTemplateField VisitNumVariable { get; set; }
        public DatasetTemplateField VisitPlannedStudyDay { get; set; }

        //TIME Fields
        public DatasetTemplateField DateTimeVariable { get; set; } //--DTC
        public DatasetTemplateField StudyDayVariable { get; set; } //--DY
        public DatasetTemplateField TimePointNameVariable { get; set; } //--TPT
        public DatasetTemplateField TimePointNumberVariable { get; set; } //--TPTNUM
        public DatasetTemplateField DurationVariable { get; set; } //--DUR

        public DatasetTemplateField StartDateTimeVariable { get; set; } //--STDTC
        public DatasetTemplateField EndDateTimeVariable { get; set; } //--ENDTC

        public DatasetTemplateField StartStudyDayVariable { get; set; } //--STDY
        public DatasetTemplateField EndStudyDayVariable { get; set; } //--ENDY

        //SUBJECT SPECIFIC Fields
        //public DatasetTemplateField ArmVariable { get; set; }
        //public DatasetTemplateField ArmCodeVariable { get; set; }
        //public DatasetTemplateField RefStartDate { get; set; }
        //public DatasetTemplateField RefEndDate { get; set; }
        //public DatasetTemplateField SiteIdVariable { get; set; }

        public bool ObsIsAFinding { get; set; } = false;
        public bool ObsIsAnEvent { get; set; } = false;
        public bool ObsIsMedDRAcoded { get; set; }
        public SdtmMedDRADescriptors MedDRAFields { get; set; }


        public List<DatasetTemplateField> MandatoryFields { get; set; }




        //public Dictionary<string, DatasetTemplateField> name2variable { get; set; }

        public static SdtmRowDescriptor GetSdtmRowDescriptor(DatasetTemplate dataset)
        {
            var descriptor = new SdtmRowDescriptor();

            descriptor.Class = dataset.Class;
            descriptor.Domain = dataset.Domain;
            descriptor.DomainCode = dataset.Code;

            descriptor.ObsIsAFinding = descriptor.Class.ToUpper() == ("FINDINGS");
            descriptor.ObsIsAnEvent = descriptor.Class.ToUpper() == "EVENTS";

            //IDENTIFIERS
            descriptor.StudyIdentifierVariable =
                dataset.Fields.Single(v => v.Name == "STUDYID");
            descriptor.DomainVariable =
               dataset.Fields.Single(v => v.Name == "DOMAIN");
            descriptor.SubjIdVariable =
                dataset.Fields.SingleOrDefault(v => v.Name == "SUBJID");
            descriptor.UniqueSubjIdVariable =
               dataset.Fields.Single(v => v.Name == "USUBJID");
            descriptor.SampleIdVariable =
               dataset.Fields.SingleOrDefault(v => v.Name.EndsWith("REFID"));

            //TOPIC DESCRIPTORS
            descriptor.TopicVariable = dataset.Fields
                        .FirstOrDefault(v => v.RoleId == "CL-Role-T-2");

            descriptor.TopicCVtermVariable = dataset.Fields.FirstOrDefault(v => v.Name == descriptor.DomainCode + "DECOD");
            if (dataset.Class.ToLower().Equals("findings"))
                descriptor.TopicCVtermVariable = dataset.Fields.FirstOrDefault(v => v.Name == descriptor.DomainCode + "LOINC");

            descriptor.TopicSynonymVariable = dataset.Fields.FirstOrDefault(v => v.Name == descriptor.DomainCode + "MODIFY");
            if (dataset.Class.ToLower().Equals("findings") || dataset.Code.ToLower().Equals("bs"))
                descriptor.TopicSynonymVariable = dataset.Fields.FirstOrDefault(v => v.Name == descriptor.DomainCode + "TEST");


            //GROUP DESCRIPTORS
            descriptor.GroupVariable = dataset.Fields.FirstOrDefault(v => v.Name == descriptor.DomainCode + "CAT");
            descriptor.SubgroupVariable = dataset.Fields.FirstOrDefault(v => v.Name == descriptor.DomainCode + "SCAT");


            //QUALIFIERS
            descriptor.QualifierFields = dataset.Fields
                        
                        .Where(v => v.RoleId == "CL-Role-T-3")
                        .ToList();
            descriptor.SynonymFields = dataset.Fields
                       
                        .Where(v => v.RoleId == "CL-Role-T-4")
                        .ToList();
            descriptor.VariableQualifierFields = dataset.Fields
                       
                        .Where(v => v.RoleId == "CL-Role-T-5")
                        .ToList();
            descriptor.ResultFields = dataset.Fields
                        
                        .Where(v => v.RoleId == "CL-Role-T-8")
                        .ToList();
            descriptor.TimeFields = dataset.Fields
                      
                        .Where(v => v.RoleId == "CL-Role-T-6")
                        .ToList();

            //MedDRAFields
            //if (descriptor.ObsIsAnEvent)
            //{
            //    descriptor.MedDRAFields = SdtmMedDRADescriptors.GetSdtmMedDRADescriptors(dataset);
            //    // descriptor.ObsIsMedDRAcoded = SdtmMedDRADescriptors.
            //}

            //VISIT
            descriptor.VisitNameVariable =
                dataset.Fields.SingleOrDefault(v => v.Name == "VISIT");
            descriptor.VisitNumVariable =
                dataset.Fields.SingleOrDefault(v => v.Name == "VISITNUM");
            descriptor.VisitPlannedStudyDay =
                dataset.Fields.SingleOrDefault(v => v.Name == "VISITDY");

            //DATETIME
            descriptor.DateTimeVariable =
                dataset.Fields.SingleOrDefault(v => v.Name == descriptor.DomainCode + "DTC");

            //STUDYDAY
            descriptor.StudyDayVariable =
               dataset.Fields.SingleOrDefault(v => v.Name == descriptor.DomainCode + "DY");

            //TIMEPOINT
            descriptor.TimePointNameVariable =
                dataset.Fields.SingleOrDefault(v => v.Name == descriptor.DomainCode + "TPT");
            descriptor.TimePointNumberVariable =
               dataset.Fields.SingleOrDefault(v => v.Name == descriptor.DomainCode + "TPTNUM");

            //DURATION (Collected NOT derived)
            descriptor.DurationVariable =
                dataset.Fields.SingleOrDefault(v => v.Name == descriptor.DomainCode + "DUR");

            //START DATETIME
            descriptor.StartDateTimeVariable =
                dataset.Fields.SingleOrDefault(v => v.Name == descriptor.DomainCode + "STDTC");
            //END DATETIME
            descriptor.EndDateTimeVariable =
               dataset.Fields.SingleOrDefault(v => v.Name == descriptor.DomainCode + "ENDTC");
            //START STUDY DAY
            descriptor.StartStudyDayVariable =
               dataset.Fields.SingleOrDefault(v => v.Name == descriptor.DomainCode + "STDY");
            //END STUDY DAY
            descriptor.EndStudyDayVariable =
               dataset.Fields.SingleOrDefault(v => v.Name == descriptor.DomainCode + "ENDY");


            ////DEMOGRAPHICS SPECIFIC Fields
            ////ARM
            //descriptor.ArmVariable = dataset.Fields.SingleOrDefault(v => v.DatasetTemplateField.Name == "ARM")?.DatasetTemplateField;
            ////ARMCODE
            //descriptor.ArmCodeVariable = dataset.Fields.SingleOrDefault(v => v.DatasetTemplateField.Name == "ARMCD")?.DatasetTemplateField;
            ////Subject Reference Start Date
            //descriptor.RefStartDate = dataset.Fields.SingleOrDefault(v => v.DatasetTemplateField.Name == "RFSTDTC")?.DatasetTemplateField;
            ////Reference End Date
            //descriptor.RefEndDate = dataset.Fields.SingleOrDefault(v => v.DatasetTemplateField.Name == "RFENDTC")?.DatasetTemplateField;
            ////SITE ID
            //descriptor.SiteIdVariable = dataset.Fields.SingleOrDefault(v => v.DatasetTemplateField.Name == "SITEID")?.DatasetTemplateField;

            return descriptor;
        }

        public DatasetTemplateField GetDefaultQualifier(IEnumerable<SdtmRow> observations)
        {
            const string numResVar = "STRESN";
            const string charResVar = "STRESC";
            const string oriResVar = "ORRES";
            const string occurVar = "SEV";
            
            if (ObsIsAFinding)
            {
                List<string> x;
                string s;
                if(observations.FirstOrDefault().ResultQualifiers.TryGetValue(DomainCode + numResVar, out s))

                    if (!observations.Select(o => o.ResultQualifiers[DomainCode + numResVar]).All(y => string.IsNullOrEmpty(y)))


                // x =.ToList();

                //if (observations.FirstOrDefault().ResultQualifiers.TryGetValue(DomainCode + charResVar, out s))
                //observations.Select(o => o.ResultQualifiers[DomainCode + charResVar] != "");
                //if (sdtmRow.ResultQualifiers.TryGetValue(DomainCode+numResVar, out s) && sdtmRow.ResultQualifiers[DomainCode + numResVar] != "")
                //{
                    return ResultFields.Find(rv => rv.Name.Equals(DomainCode + numResVar));
                //}
                if (observations.FirstOrDefault().ResultQualifiers.TryGetValue(DomainCode+charResVar, out s) &&

                    (!observations.Select(o => o.ResultQualifiers[DomainCode + charResVar]).All(y => string.IsNullOrEmpty(y))))

                //    && sdtmRow.ResultQualifiers[DomainCode + charResVar] != "")
                //{
                   return ResultFields.Find(rv => rv.Name.Equals(DomainCode + charResVar));
                //}
                //else
                //{
                   return ResultFields.Find(rv => rv.Name.Equals(DomainCode + oriResVar));
                //}
            }
            if (ObsIsAnEvent)
            {
                return QualifierFields.Find(rv => rv.Name.Equals(DomainCode + occurVar));
            }
            return null;
        }

        public List<DatasetTemplateField> GetAllTimingFields()
        {
            var list = new List<DatasetTemplateField>();
            list.AddRange(new List<DatasetTemplateField>() {VisitNameVariable,VisitNumVariable,VisitPlannedStudyDay,StudyDayVariable,DateTimeVariable});
            return list;
        }
    }

}

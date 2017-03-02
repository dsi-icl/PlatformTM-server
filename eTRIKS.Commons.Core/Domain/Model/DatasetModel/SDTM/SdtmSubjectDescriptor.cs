using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eTRIKS.Commons.Core.Domain.Model.DatasetModel.SDTM
{
    public class SdtmSubjectDescriptor : SdtmRowDescriptor
    {
        public VariableDefinition ArmVariable { get; set; }
        public VariableDefinition ArmCodeVariable { get; set; }

        public VariableDefinition ActArmVariable { get; set; }
        public VariableDefinition ActArmCodeVariable { get; set; }

        public VariableDefinition RefStartDate { get; set; }
        public VariableDefinition RefEndDate { get; set; }

        public VariableDefinition TreatmentStartDate { get; set; }
        public VariableDefinition TreatmentEndDate { get; set; }

        public VariableDefinition InformedConsentDate { get; set; }

        public VariableDefinition EndOfParticipationDate { get; set; }

        public VariableDefinition SiteIdVariable { get; set; }

        public List<VariableDefinition> CharacteristicProperties { get; set; }

        public static SdtmSubjectDescriptor GetSdtmSubjectDescriptor(Dataset dataset)
        {
            var descriptor = new SdtmSubjectDescriptor();

            descriptor.Class = dataset.Template.Class;
            descriptor.Domain = dataset.Template.Domain;
            descriptor.DomainCode = dataset.Template.Code;

            //IDENTIFIERS
            descriptor.StudyIdentifierVariable =
                dataset.Variables.Single(v => v.VariableDefinition.Name == "STUDYID").VariableDefinition;
            descriptor.DomainVariable =
               dataset.Variables.Single(v => v.VariableDefinition.Name == "DOMAIN").VariableDefinition;
            descriptor.SubjIdVariable =
                dataset.Variables.SingleOrDefault(v => v.VariableDefinition.Name == "SUBJID")?.VariableDefinition;
            descriptor.UniqueSubjIdVariable =
               dataset.Variables.Single(v => v.VariableDefinition.Name == "USUBJID").VariableDefinition;


            //Subject Reference Start Date
            descriptor.RefStartDate = dataset.Variables.SingleOrDefault(v => v.VariableDefinition.Name == "RFSTDTC")?.VariableDefinition;
            //Reference End Date
            descriptor.RefEndDate = dataset.Variables.SingleOrDefault(v => v.VariableDefinition.Name == "RFENDTC")?.VariableDefinition;

            descriptor.TreatmentStartDate = dataset.Variables.SingleOrDefault(v => v.VariableDefinition.Name == "RFXSTDTC")?.VariableDefinition;
            descriptor.TreatmentStartDate = dataset.Variables.SingleOrDefault(v => v.VariableDefinition.Name == "RFXENDTC")?.VariableDefinition;

            descriptor.EndOfParticipationDate = dataset.Variables.SingleOrDefault(v => v.VariableDefinition.Name == "RFPENDTC")?.VariableDefinition;
            descriptor.InformedConsentDate = dataset.Variables.SingleOrDefault(v => v.VariableDefinition.Name == "RFICDTC")?.VariableDefinition;

            //ARM
            descriptor.ArmVariable = dataset.Variables.SingleOrDefault(v => v.VariableDefinition.Name == "ARM")?.VariableDefinition;
            //ARMCODE
            descriptor.ArmCodeVariable = dataset.Variables.SingleOrDefault(v => v.VariableDefinition.Name == "ARMCD")?.VariableDefinition;

            //ACTUAL ARM
            descriptor.ActArmVariable = dataset.Variables.SingleOrDefault(v => v.VariableDefinition.Name == "ACTARM")?.VariableDefinition;
            //ACTUAL ARMCODE
            descriptor.ActArmCodeVariable = dataset.Variables.SingleOrDefault(v => v.VariableDefinition.Name == "ACTARMCD")?.VariableDefinition;

            //SITE ID
            descriptor.SiteIdVariable = dataset.Variables.SingleOrDefault(v => v.VariableDefinition.Name == "SITEID")?.VariableDefinition;

            descriptor.CharacteristicProperties.AddRange(new List<VariableDefinition>()
            {
                dataset.Variables.SingleOrDefault(v => v.VariableDefinition.Name == "BRTHDTC")?.VariableDefinition,
                dataset.Variables.SingleOrDefault(v => v.VariableDefinition.Name == "AGE")?.VariableDefinition,
                dataset.Variables.SingleOrDefault(v => v.VariableDefinition.Name == "SEX")?.VariableDefinition,
                dataset.Variables.SingleOrDefault(v => v.VariableDefinition.Name == "RACE")?.VariableDefinition,
                dataset.Variables.SingleOrDefault(v => v.VariableDefinition.Name == "ETHNIC")?.VariableDefinition
            });
            
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
           

            
            return descriptor;
        }
    }
}

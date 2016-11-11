using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTRIKS.Commons.Core.Domain.Model.DatasetModel.SDTM
{
    public class SdtmMedDRADescriptors
    {
        public VariableDefinition LowesLevelTerm_variable { get; set; }
        public VariableDefinition LowestLevelTermCode_variable { get; set; }
        public VariableDefinition PreferredTerm_variable { get; set; }
        public VariableDefinition PreferredTermCode_variable { get; set; }
        public VariableDefinition HighLevelTerm_variable { get; set; }
        public VariableDefinition HighLevelTermCode_variable { get; set; }
        public VariableDefinition HighLevelGroupTerm_variable { get; set; }
        public VariableDefinition HighLevelGroupTermCode_variable { get; set; }
        public VariableDefinition SystemOrganClass_variable { get; set; }
        public VariableDefinition SystemOrganClassCode_variable { get; set; }

        public static SdtmMedDRADescriptors GetSdtmMedDRADescriptors(Dataset dataset)
        {
            var meddraDescriptors = new SdtmMedDRADescriptors();
            meddraDescriptors.LowesLevelTerm_variable = dataset.Variables.SingleOrDefault(v => v.VariableDefinition.Name == "AELLT")?.VariableDefinition;
            meddraDescriptors.LowestLevelTermCode_variable = dataset.Variables.SingleOrDefault(v => v.VariableDefinition.Name == "AELLTCD")?.VariableDefinition;
            meddraDescriptors.PreferredTerm_variable = dataset.Variables.SingleOrDefault(v => v.VariableDefinition.Name == "AEDECOD")?.VariableDefinition;
            meddraDescriptors.PreferredTermCode_variable = dataset.Variables.SingleOrDefault(v => v.VariableDefinition.Name == "AEPTCD")?.VariableDefinition;
            meddraDescriptors.HighLevelTerm_variable = dataset.Variables.SingleOrDefault(v => v.VariableDefinition.Name == "AEHLT")?.VariableDefinition;
            meddraDescriptors.HighLevelTermCode_variable = dataset.Variables.SingleOrDefault(v => v.VariableDefinition.Name == "AEHLTCD")?.VariableDefinition;
            meddraDescriptors.HighLevelGroupTerm_variable = dataset.Variables.SingleOrDefault(v => v.VariableDefinition.Name == "AEHLGT")?.VariableDefinition;
            meddraDescriptors.HighLevelGroupTermCode_variable = dataset.Variables.SingleOrDefault(v => v.VariableDefinition.Name == "AEHLGTCD")?.VariableDefinition;
            meddraDescriptors.SystemOrganClass_variable = dataset.Variables.SingleOrDefault(v => v.VariableDefinition.Name == "AESOC")?.VariableDefinition;
            meddraDescriptors.SystemOrganClassCode_variable = dataset.Variables.SingleOrDefault(v => v.VariableDefinition.Name == "AESOCCD")?.VariableDefinition;

            return meddraDescriptors;
        }

        
    }
}

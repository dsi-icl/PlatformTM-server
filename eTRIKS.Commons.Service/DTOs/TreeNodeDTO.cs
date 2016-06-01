using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eTRIKS.Commons.Service.Services;

namespace eTRIKS.Commons.Service.DTOs
{
    public class TreeNodeDTO
    {
        public string Id { get; set; }
        public List<TreeNodeDTO> Children { get; set; }
        public string Text { get; set; }
        public Field Field { get; set; }
        public bool IsGroup { get; set; }
        public string Type { get; set; }
    }

    public class Field
    {
        public string O3Name { get; set; } //INJECTION SITE PAIN // BMI
        public int O3Id { get; set; }
        public string O3VarName { get; set; } //AEDECOD // VSTESTCD
        public string QO2Name { get; set; } //AESEV // VSORRES (what will be added as a projection field)
        public ValueSetDTO ValueSet { get; set; }
        private string _displayName;
        public string DisplayName
        {
            get { return Domain+" - "+( GroupName!=null?(GroupName+" - "):"") +O3Name + " [" + QO2Name + "]"; }
            set { _displayName = value; }
        }
        public string Id { get; set; }
        public string DomainCode { get; set; }

        public string GroupName { get; set; }

        public string GroupVarName { get; set; }


        public string Domain { get; set; }
    }

    public class ValueSetDTO
    {
        public bool IsNumeric { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }
        public double From { get; set; }
        public double To { get; set; }
        public List<string> FilterValues { get; set; } 
        public List<string> ValueSet { get; set; }
        public string Unit { get; set; }
    }
}

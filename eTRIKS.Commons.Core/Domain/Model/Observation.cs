using eTRIKS.Commons.Core.Domain.Model.Base;
using eTRIKS.Commons.Core.Domain.Model.ControlledTerminology;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTRIKS.Commons.Core.Domain.Model
{
    public class Observation : Identifiable<int>
    {
        //Identifier variables
        
        public string Name { get; set; } //BMI
        public string ControlledTermStr { get; set; }
        public CVterm ControlledTerm { get; set; }
        public String ControlledTermId { get; set; }
        public string DomainCode { get; set; } //VS
        public VariableDefinition TopicVariable { get; set; } //VSTESTCD
        public int TopicVariableId { get; set; }
        public string Class { get; set; } //Findings
        public string Group { get; set; } //null //shuold be variable not string?
        public string Subgroup { get; set; } //null 
        public VariableDefinition DefaultQualifier { get; set; }
        public int? DefaultQualifierId { get; set; }
        public ICollection<Study> Studies { get; set; }

        public List<VariableDefinition> Synonyms { get; set; }
        public List<VariableDefinition> Qualifiers { get; set; } //VSORES, VSLOC ...etc
        public List<VariableDefinition> Timings { get; set; } //VISIT VSDY VSSTD

        public Observation()
        {
            Studies = new List<Study>();
            Synonyms = new List<VariableDefinition>();
            Qualifiers = new List<VariableDefinition>();
            Timings = new List<VariableDefinition>();
        }
    }
}

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
        public string DomainName { get; set; } //Vital Signs

        //TODO: should be changed to a reference to the Observation 
        //or replaced by an "Observation Identity" variable to represent the OBJECT of Observation
        public VariableDefinition TopicVariable { get; set; } //VSTESTCD // ... NO NEED for this now since we dont query for data from mongo
        //this used to be the link between the SQL and noSQL databases to know which variable we should query for to bring back the data
        public int TopicVariableId { get; set; }
        public string Class { get; set; } //Findings
        public string Group { get; set; } //null //shuold be variable not string?
        public string Subgroup { get; set; } //null 
        public VariableDefinition DefaultQualifier { get; set; }
        public int? DefaultQualifierId { get; set; }
        public List<Study> Studies { get; set; }
        public bool? isSubjCharacteristic { get; set; }

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

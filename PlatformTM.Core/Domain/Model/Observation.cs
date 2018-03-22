using System;
using System.Collections.Generic;
using PlatformTM.Core.Domain.Model.Base;
using PlatformTM.Core.Domain.Model.ControlledTerminology;
using PlatformTM.Core.Domain.Model.DatasetModel;
using PlatformTM.Core.JoinEntities;

namespace PlatformTM.Core.Domain.Model
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
        //public List<Study> Studies { get; set; }
        public bool? isSubjCharacteristic { get; set; }
        public int? DatasetId { get; set; }
        public Dataset Dataset { get; set; }
        public DataFile Datafile { get; set; }
        public int? DatafileId { get; set; }
        public Project Project { get; set; }
        public int? ProjectId { get; set; }

        public List<ObservationSynonym> Synonyms { get; set; }
        public virtual List<ObservationQualifier> Qualifiers { get; set; } //VSORES, VSLOC ...etc
        public virtual List<ObservationTiming> Timings { get; set; } //VISIT VSDY VSSTD

        public Observation()
        {
            //Studies = new List<Study>();
            Synonyms = new List<ObservationSynonym>();
            Qualifiers = new List<ObservationQualifier>();
            Timings = new List<ObservationTiming>();
        }
    }
}

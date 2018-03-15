using System;
using System.Collections.Generic;
using PlatformTM.Core.Domain.Model.Base;
using PlatformTM.Core.Domain.Model.Timing;

namespace PlatformTM.Core.Domain.Model
{
    public class SubjectObservation : Identifiable<Guid>
    {
        public string SubjectId { get; set; }
        public string StudyId { get; set; }
        public string Name { get; set; }// should be the Object of Observation entity with and id ...etc
        public string VerbatimName { get; set; }
        public string StandardName { get; set; }
        public string Class { get; set; } //Findings
        public string DomainCode { get; set; }
        public string DomainName { get; set; }
        public string Group { get; set; } //null //shuold be variable not string?
        public string Subgroup { get; set; } //null 
        public string Visit { get; set; }
        public int VisitNum { get; set; }
        public Dictionary<string, string> qualifiers { get; set; } //  That becomes an array of maps
        public Dictionary<string, string> timings { get; set; }
        public AbsoluteTimePoint ObDateTime { get; set; } //Date of Collection / --DTC
        public RelativeTimePoint ObsStudyDay { get; set; } //--DY
        public RelativeTimePoint ObsStudyTimePoint { get; set; } //--TPT
        public TimeInterval ObsInterval { get; set; }

        public SubjectObservation()
        {
            qualifiers = new Dictionary<string, string>();
            timings = new Dictionary<string, string>();
        }
        public class ObsQualifier
        {
            enum type
            {

            }
            public string fieldName { get; set; }
            public string value { get; set; }
        }

        public string getNameVariable()
        {
            return "VSTESTCD";
        }
        




        public int ActivityId { get; set; }

        public int DBstudyId { get; set; }

        public int DatafileId { get; set; }

        public int ProjectId { get; set; }

        public int DatasetId { get; set; }

        public string ProjectAcc { get; set; }
        public int O3Id { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Model.Base;
using eTRIKS.Commons.Core.Domain.Model.Timing;

namespace eTRIKS.Commons.Core.Domain.Model
{
    /*****
     * SDTMENTITY is the generic subject matter than a particular SDTM is collecting observations about
     * In case of DM the subject matter (SDTMentity) is the Subject
     * In case of Findigs the TEST is the subject matter
     * In case of Events the event TERM is the subject matter
     * In case of Intervatntions the TRT is the subject matter
     */
    public class SdtmEntity : Identifiable<Guid>
    {
        public DateTime RFENDTC;
        public DateTime RFSTDTC;
        //public Subject Subject {get; set; }
        public string SubjectId { get; set; }
        public string USubjId { get; set; }
        public string SampleId { get; set; }
        public string StudyId { get; set; }
        //public Dictionary<string, string> Characteristics { get; set; }
        public int ActivityId { get; set; }
        public int DatasetId { get; set; }
        //public string Visit { get; set; }
        //public int VisitNum { get; set; }
        public Dictionary<string, string> Qualifiers { get; set; }
        public Dictionary<string, string> QualifierQualifiers { get; set; }
        public Dictionary<string, string> QualifierSynonyms { get; set; }
        //public ICollection<Characterisitc> Characteristics { get; set; }
        public Visit Visit { get; set; }
        public AbsoluteTimePoint CollectionDateTime { get; set; } //Date of Collection / --DTC
        public RelativeTimePoint CollectionStudyDay { get; set; } //--DY
        public RelativeTimePoint CollectionStudyTimePoint { get; set; } //--TPT
        //public TimeInterval ObsInterval { get; set; }
        public string DomainCode { get; set; }
        public string Arm { get; set; }
        public string ArmCode { get; set; }
        public string SiteId { get; set; }
        public Dictionary<string, string> Groups { get; set; }
        public Dictionary<string, string> Leftovers { get; set; }

        public SdtmEntity()
        {
            Qualifiers = new Dictionary<string, string>();
            QualifierSynonyms = new Dictionary<string, string>();
            QualifierQualifiers = new Dictionary<string, string>();
            Leftovers = new Dictionary<string, string>();
            Groups = new Dictionary<string, string>();
            Visit = new Visit();
        }



        public int ProjectId { get; set; }
        public int DBstudyId { get; set; }

        public string VerbatimStudyId { get; set; }

        public int DatafileId { get; set; }
    }
}

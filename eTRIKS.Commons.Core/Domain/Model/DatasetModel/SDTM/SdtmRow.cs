using System;
using System.Collections.Generic;
using eTRIKS.Commons.Core.Domain.Model.Base;
using eTRIKS.Commons.Core.Domain.Model.Timing;

namespace eTRIKS.Commons.Core.Domain.Model.DatasetModel.SDTM
{
    /*****
     * SDTMENTITY is the generic subject matter than a particular SDTM is collecting observations about
     * In case of DM the subject matter (SDTMentity) is the Subject
     * In case of Findigs the TEST is the subject matter
     * In case of Events the event TERM is the subject matter
     * In case of Intervatntions the TRT is the subject matter
     */
    public class SdtmRow : Identifiable<Guid>
    {  
        public string StudyId { get; set; }
        public string DomainCode { get; set; }
        public string Class { get; set; }
        public string SubjectId { get; set; }
        public string USubjId { get; set; }
        public string SampleId { get; set; }

        public string TopicSynonym { get; set; }
        public string Topic { get; set; }
        public string TopicControlledTerm { get; set; }

        public string Group { get; set; }
        public string Subgroup { get; set; }

        public Dictionary<string, string> Qualifiers { get; set; }
        public Dictionary<string, string> ResultQualifiers { get; set; }
        public Dictionary<string, string> QualifierQualifiers { get; set; }
        public Dictionary<string, string> QualifierSynonyms { get; set; }
        public Dictionary<string, string> TimingQualifiers { get; set; }

        public AbsoluteTimePoint CollectionDateTime { get; set; } //Date of Collection / --DTC
        public RelativeTimePoint CollectionStudyDay { get; set; } //--DY
        public RelativeTimePoint CollectionStudyTimePoint { get; set; } //--TPT
        public TimeInterval StudyDayInterval { get; set; }
        public TimeInterval DateTimeInterval { get; set; }
        public string Duration { get; set; }



        //VISIT varaibles
        public string VisitName { get; set; }
        public int VisitNum { get; set; }
        public int VisitPlannedStudyDay { get; set; }

        public Dictionary<string, string> Leftovers { get; set; }

        public bool BaseLineFlag { get; set; } //--BLFL


        //SQL DB RELATIONSHIPS
        public int ActivityId { get; set; }
        public int DatasetId { get; set; }
        public int DBStudyId { get; set; }
        public int DatafileId { get; set; }
        public int ProjectId { get; set; }
        public string ProjectAccession { get; set; }
        public int DBTopicId { get; set; }

        public Dictionary<string, string> VarNameToProperty { get; set; }

        public SdtmRow()
        {
            Qualifiers = new Dictionary<string, string>();
            ResultQualifiers = new Dictionary<string, string>();
            QualifierSynonyms = new Dictionary<string, string>();
            QualifierQualifiers = new Dictionary<string, string>();
            TimingQualifiers = new Dictionary<string, string>();
            Leftovers = new Dictionary<string, string>();
        }

        public override bool Equals(Object o)
        {
            // If parameter is null return false.
            if (o == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            SdtmRow r = o as SdtmRow;
            

            // Return true if the fields match:
            return (Id == r.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}

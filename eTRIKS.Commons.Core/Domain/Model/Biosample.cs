using System.Collections.ObjectModel;
using eTRIKS.Commons.Core.Domain.Model.Base;
using System.Collections.Generic;
using eTRIKS.Commons.Core.Domain.Model.Timing;
using eTRIKS.Commons.Core.Domain.Model.DesignElements;

namespace eTRIKS.Commons.Core.Domain.Model
{
    public class Biosample : Identifiable<int>
    {
        public HumanSubject Subject {get; set; }
        public string SubjectId { get; set; }
        public string BiosampleStudyId { get; set; }
        public Study Study { get; set; }
        public int StudyId { get; set; }
        public Assay Assay { get; set; }
        public int AssayId { get; set; }
        public Dataset Dataset { get; set; }
        public int DatasetId { get; set; }
        public bool? IsBaseline { get; set; }
        public ICollection<SampleCharacteristic> SampleCharacteristics { get; set; }
        public Visit Visit { get; set; }
        public int? VisitId { get; set; }
        public AbsoluteTimePoint CollectionDateTime { get; set; } //Date of Collection / --DTC
        public RelativeTimePoint CollectionStudyDay { get; set; } //--DY
        public RelativeTimePoint CollectionStudyTimePoint { get; set; } //--TPT
        //public TimeInterval ObsInterval { get; set; }

        public Biosample()
        {   
            SampleCharacteristics = new Collection<SampleCharacteristic>();
        }
    }
}

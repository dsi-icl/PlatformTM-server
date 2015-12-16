using eTRIKS.Commons.Core.Domain.Model.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Model.Timing;

namespace eTRIKS.Commons.Core.Domain.Model
{
    public class Biospecimen : Identifiable<Guid>
    {
        public string SubjectId { get; set; }
        public string SampleId { get; set; }
        public string StudyId { get; set; }
        public Dictionary<string, string> Characteristics { get; set; }
        public int AssayId { get; set; }
        public string Visit { get; set; }
        public int VisitNum { get; set; }
        public Dictionary<string, string> Timings { get; set; }
        public AbsoluteTimePoint ObDateTime { get; set; } //Date of Collection / --DTC
        public RelativeTimePoint ObsStudyDay { get; set; } //--DY
        public RelativeTimePoint ObsStudyTimePoint { get; set; } //--TPT
        public TimeInterval ObsInterval { get; set; }
        public string DomainCode { get; set; }

        public Biospecimen()
        {
            Characteristics = new Dictionary<string, string>();
            Timings = new Dictionary<string, string>();
        }
    }
}

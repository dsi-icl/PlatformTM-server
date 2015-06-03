using eTRIKS.Commons.Core.Domain.Model.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTRIKS.Commons.Core.Domain.Model
{
    public class Subject : Identifiable<Guid>
    {
        public Study Study { get; set; }
        public string StudyId { get; set; }
        public string SubjId { get; set; }
        public string Site { get; set; }
        public DateTime StudyStartDate { get; set; }
        public DateTime StudyEndDate { get; set; }
        public string Arm { get; set; } //Should be replace by ARM class reference
        public string ArmCode { get; set; } //Should be replace by ARM class reference
        public string DomainCode { get; set; }
        public List<Observation> characteristics {get;set;}
        public Dictionary<string, string> characteristicsValues { get; set; }

        public Subject()
        {
            characteristicsValues = new Dictionary<string, string>();
        }
    }
}

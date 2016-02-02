using eTRIKS.Commons.Core.Domain.Model.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTRIKS.Commons.Core.Domain.Model
{
    public class Subject : Identifiable<string>
    {
        public Study Study { get; set; }
        public int StudyId { get; set; }
        public string SubjectStudyId { get; set; }
        public string UniqueSubjectId { get; set; }
        //public string Site { get; set; }
        public DateTime SubjectStartDate { get; set; }
        public DateTime SubjectEndDate { get; set; }
        public string Arm { get; set; } //Should be replace by ARM class reference
        public string ArmCode { get; set; } //Should be replace by ARM class reference
        public Dataset Dataset { get; set; }
        public int DatasetId { get; set; }
        public ICollection<SubjectCharacteristic> SubjectCharacteristics { get; set; }

        public Subject()
        {
            SubjectCharacteristics = new List<SubjectCharacteristic>();
        }
    }
}

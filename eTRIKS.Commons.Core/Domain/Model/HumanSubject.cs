using eTRIKS.Commons.Core.Domain.Model.Base;
using System;
using System.Collections.Generic;
using eTRIKS.Commons.Core.Domain.Model.DatasetModel;
using eTRIKS.Commons.Core.Domain.Model.DesignElements;

namespace eTRIKS.Commons.Core.Domain.Model
{
    public class HumanSubject : Identifiable<string>
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
        public Arm StudyArm { get; set; }
        public string StudyArmId { get; set; }
        public Dataset Dataset { get; set; }
        public int DatasetId { get; set; }
        public ICollection<SubjectCharacteristic> SubjectCharacteristics { get; set; }

        public HumanSubject()
        {
            SubjectCharacteristics = new List<SubjectCharacteristic>();
        }
    }
}

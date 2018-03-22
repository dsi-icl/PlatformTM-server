using System;
using System.Collections.Generic;
using PlatformTM.Core.Domain.Model.Base;
using PlatformTM.Core.Domain.Model.DatasetModel;
using PlatformTM.Core.Domain.Model.DesignElements;

namespace PlatformTM.Core.Domain.Model
{
    public class HumanSubject : Identifiable<string>
    {
        public string SubjectStudyId { get; set; }
        public string UniqueSubjectId { get; set; }

        public Study Study { get; set; }
        public int StudyId { get; set; }
        
        public DateTime SubjectStartDate { get; set; }
        public DateTime SubjectEndDate { get; set; }

        public string Arm { get; set; } //Should be replace by ARM class reference
        public string ArmCode { get; set; } //Should be replace by ARM class reference

        public Arm StudyArm { get; set; }
        public string StudyArmId { get; set; }

        public Dataset Dataset { get; set; }
        public int DatasetId { get; set; }

        public IList<SubjectCharacteristic> SubjectCharacteristics { get; set; }

        public DataFile SourceDataFile { get; set; }
        public int? DatafileId { get; set; }

        public HumanSubject()
        {
            SubjectCharacteristics = new List<SubjectCharacteristic>();
        }
    }
}

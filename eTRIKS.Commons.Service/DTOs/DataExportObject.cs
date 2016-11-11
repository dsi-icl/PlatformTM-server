using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Core.Domain.Model.DesignElements;
using eTRIKS.Commons.Core.Domain.Model.DatasetModel.SDTM;

namespace eTRIKS.Commons.Service.DTOs
{
    public class DataExportObject
    {
        private readonly IRepository<Arm, string> _armRepository;
        private readonly IRepository<SubjectCharacteristic, int> _subjectCharacteristicRepository;
        private readonly IRepository<Study, int> _studyRepository;
        private readonly IRepository<HumanSubject, string> _subjectRepository;

        public List<SdtmRow> Observations { get; set; }
        public List<SubjectCharacteristic> SubjChars { get; set; }
        public List<Visit> Visits { get; set; }
        public List<Arm> Arms { get; set; }
        public List<Study> Studies { get; set; }
        public List<HumanSubject> SubjectArms { get; set; }
        public List<Study> StudyArms { get; internal set; }
        public bool IsSubjectIncluded { get; set; }

        public DataExportObject()
        {
            Observations = new List<SdtmRow>();
            SubjChars = new List<SubjectCharacteristic>();
            Visits = new List<Visit>();
            Arms = new List<Arm>();
            Studies = new List<Study>();

            //_subjectCharacteristicRepository = uoW.GetRepository<SubjectCharacteristic, int>();
            //_studyRepository = uoW.GetRepository<Study, int>();
            //_armRepository = uoW.GetRepository<Arm, string>();
            //_subjectRepository = uoW.GetRepository<HumanSubject, string>();
        }

        public IEnumerable JoinSubjObs()
        {
            return Observations.Join(SubjChars, o => o.SubjectId, s => s.SubjectId, (o, s) => new {o, s}).ToList();
        }



        public string GetArmForSubject(string subjectId)
        {
            return SubjectArms.Find(a => a.Id == subjectId)?.StudyArm.Name;
        }

        public string GetSubjCharacterisiticForSubject(string subjectId, int characteristicId)
        {
            return
                SubjChars.Find(sc => sc.SubjectId == subjectId && sc.CharacteristicObjectId == characteristicId)?
                    .VerbatimValue;
        }

        public string GetStudyForSubject(string subjectId)
        {
            return Studies.Find(s => s.Subjects.Select(j => j.UniqueSubjectId).Contains(subjectId)).Name;
        }

        //public void FillArms(string projectAcc)
        //{
        //    var arms = _armRepository.FindAll(a => a.Studies.All(s => s.Project.Accession == projectAcc)).ToList();
        //    //Apply filter?
        //   Arms = arms;
        //    SubjectArms = _subjectRepository.FindAll(s => s.Study.Project.Accession == projectAcc,
        //        new List<Expression<Func<HumanSubject, object>>>() { s => s.StudyArm }).ToList();
        //    StudyArms = _studyRepository.FindAll(s => s.Project.Accession == projectAcc,
        //        new List<Expression<Func<Study, object>>>() { s => s.Arms }).ToList();
        //}
    }


}

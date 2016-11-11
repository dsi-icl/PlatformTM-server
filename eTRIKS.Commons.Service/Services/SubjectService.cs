using System;
using System.Collections.Generic;
using System.Linq;
using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Core.Domain.Model.DatasetModel;
using eTRIKS.Commons.Core.Domain.Model.DatasetModel.SDTM;
using eTRIKS.Commons.Core.Domain.Model.DesignElements;

namespace eTRIKS.Commons.Service.Services
{
    class SubjectService
    {
        private readonly IRepository<HumanSubject, string> _subjectRepository;
        private readonly IRepository<Study, int> _studtRepository;
        private readonly IRepository<CharacteristicObject, int> _characteristicObjRepository;

        private readonly IServiceUoW _dataContext;

        public SubjectService(IServiceUoW uoW)
        {
            _dataContext = uoW;
            _subjectRepository = uoW.GetRepository<HumanSubject, string>();
            _studtRepository = uoW.GetRepository<Study, int>();
            _characteristicObjRepository = uoW.GetRepository<CharacteristicObject, int>();
        }

        public bool LoadSubjects(List<SdtmRow> subjectData, SdtmRowDescriptor sdtmRowDescriptor)
        {
            if (subjectData.Count == 0)
                return false;

            var projectId = subjectData.First().ProjectId;
            var projectAccession = subjectData.First().ProjectAccession;

            //Project related subject characteristics
            var scoList = _characteristicObjRepository.FindAll(s => s.ProjectId == projectId).ToList();
            var scos = scoList.ToDictionary(co => co.ShortName);
            //Project related studies
            var studies = _studtRepository.FindAll(s => s.ProjectId == projectId, new List<System.Linq.Expressions.Expression<Func<Study, object>>> {s=>s.Arms });
            var studyMap = studies.ToDictionary(study => study.Name);
            //Project related studies
            var arms = studies.SelectMany(s => s.Arms);
            var armMap = arms.ToDictionary(arm => arm.Name);

            foreach (var sdtmSubject in subjectData)
            {
                Study study;
                if (!studyMap.TryGetValue(sdtmSubject.StudyId, out study))
                {
                    study = new Study() {
                        Name = sdtmSubject.StudyId,
                        Accession = "S-"+ projectAccession.Substring(2)+"-"+ (studies.Count() + 1).ToString("00"),
                        ProjectId = projectId
                    };studyMap.Add(study.Name,study);
                }
                Arm arm;
                if (!armMap.TryGetValue(sdtmSubject.QualifierSynonyms[sdtmRowDescriptor.ArmVariable.Name], out arm))
                {
                    arm = new Arm()
                    {
                        Id = projectAccession + "-" + sdtmSubject.Qualifiers[sdtmRowDescriptor.ArmCodeVariable.Name],
                        Code = sdtmSubject.Qualifiers[sdtmRowDescriptor.ArmCodeVariable.Name],
                        Name = sdtmSubject.QualifierSynonyms[sdtmRowDescriptor.ArmVariable.Name]
                    };
                    if(arm.Studies==null) arm.Studies = new List<Study>();
                    if(!arm.Studies.Exists(s=>s.Name == study.Name)) arm.Studies.Add(study);
                    armMap.Add(arm.Name,arm);
                }

                /**
                  * ADDING SUBJECTS
                  */
                 var subject = new HumanSubject
                 {
                     Id = "P-"+projectId+"-"+sdtmSubject.USubjId,
                     UniqueSubjectId = sdtmSubject.USubjId,
                     SubjectStudyId = sdtmSubject.SubjectId,
                     Arm = sdtmSubject.QualifierSynonyms[sdtmRowDescriptor.ArmVariable.Name], //Will put in characteristics for now
                     ArmCode = sdtmSubject.Qualifiers[sdtmRowDescriptor.ArmCodeVariable.Name],
                     DatasetId = sdtmSubject.DatasetId,
                     
                     Study = study,
                     StudyArm = arm
                 };

                DateTime startDate, endDate;
                if(sdtmRowDescriptor.RefStartDate != null && sdtmSubject.Qualifiers[sdtmRowDescriptor?.RefStartDate?.Name] != null)
                {
                    if (DateTime.TryParse(sdtmSubject.Qualifiers[sdtmRowDescriptor.RefStartDate?.Name], out startDate))
                        subject.SubjectStartDate = startDate;
                }

                if (sdtmRowDescriptor.RefEndDate != null && sdtmSubject.Qualifiers[sdtmRowDescriptor.RefEndDate?.Name] != null)
                {
                    if (DateTime.TryParse(sdtmSubject.Qualifiers[sdtmRowDescriptor.RefEndDate.Name], out endDate))
                        subject.SubjectEndDate = endDate;
                }
                   
               

                 /**
                  * ADDING SUBJECT CHARACTERISTICS
                  */
                 foreach (var qualifier in sdtmSubject.Qualifiers)
                 {
                     var dsVar = sdtmRowDescriptor.QualifierVariables.SingleOrDefault(v => v.Name.Equals(qualifier.Key));
                     if (dsVar != null)
                     {
                         CharacteristicObject sco;
                         scos.TryGetValue(qualifier.Key, out sco);
                         if (sco == null)
                         {
                             /**
                              * CREATING NEW CHARACTERISTIC OBJECT
                              */
                             sco = new CharacteristicObject()
                             {
                                ShortName = dsVar.Name,
                                FullName = dsVar.Label,
                                Domain = "DM",
                                ProjectId = projectId,
                                DataType = dsVar.DataType
                             };
                             scos.Add(dsVar.Name,sco);
                         }

                         /**
                         * ADDING SUBJECT CHARACTERISTIC
                         */
                         subject.SubjectCharacteristics.Add(new SubjectCharacteristic()
                         {
                             //DatasetVariable = dsVar,
                             CharacteristicObject = sco,
                             VerbatimValue = qualifier.Value,
                             VerbatimName = sco.ShortName,
                             DatasetDomainCode = sdtmSubject.DomainCode,
                             Subject = subject
                         });
                     }
                     else
                         throw new Exception("Qualifier not in dataset template");
                 }
                 _subjectRepository.Insert(subject);
             }
             return _dataContext.Save().Equals("CREATED");
        }
    }
}

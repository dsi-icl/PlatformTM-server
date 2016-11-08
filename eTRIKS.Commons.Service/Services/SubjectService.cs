using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Core.Domain.Model.Data.SDTM;

namespace eTRIKS.Commons.Service.Services
{
    class SubjectService
    {
        private readonly IRepository<HumanSubject, string> _subjectRepository;
        private readonly IRepository<Dataset, int> _datasetRepository;
        private readonly IRepository<Study, int> _studtRepository;
        private readonly IRepository<CharacteristicObject, int> _characteristicObjRepository;

        private readonly IServiceUoW _dataContext;

        public SubjectService(IServiceUoW uoW)
        {
            _dataContext = uoW;
            _subjectRepository = uoW.GetRepository<HumanSubject, string>();
            _datasetRepository = uoW.GetRepository<Dataset, int>();
            _studtRepository = uoW.GetRepository<Study, int>();
            _characteristicObjRepository = uoW.GetRepository<CharacteristicObject, int>();
        }

        public async Task<bool> LoadSubjects(List<SdtmRow> subjectData, int datasetId)
        {
            if (subjectData.Count == 0)
                return false;

            var dataset = _datasetRepository.FindSingle(d => d.Id.Equals(datasetId),
                new List<Expression<Func<Dataset, object>>>()
                {
                    d => d.Variables.Select(v=>v.VariableDefinition),
                    d => d.Activity.Project
                });

            if (dataset == null)
                return false;

            //Project related subject characteristics
            var scoList = _characteristicObjRepository.FindAll(s => s.ProjectId.Equals(dataset.Activity.ProjectId)).ToList();
            var scos = scoList.ToDictionary(co => co.ShortName);
            //Project related studies
            var studies = _studtRepository.FindAll(s => s.ProjectId == dataset.Activity.ProjectId);
            var studyMap = studies.ToDictionary(study => study.Name);

            foreach (var sdtmEntity in subjectData)
            {
                Study study;
                if (!studyMap.TryGetValue(sdtmEntity.StudyId, out study))
                {
                    study = new Study() {
                        Name = sdtmEntity.StudyId,
                        Accession = "S-"+dataset.Activity.Project.Accession.Substring(2)+"-"+ (studies.Count() + 1).ToString("00"),
                        ProjectId = dataset.Activity.ProjectId
                    };studyMap.Add(study.Name,study);
                }

                 /**
                  * ADDING SUBJECTS
                  */
                 var subject = new HumanSubject
                 {
                     Id = sdtmEntity.USubjId,
                     UniqueSubjectId = sdtmEntity.USubjId,
                     SubjectStudyId = sdtmEntity.SubjectId,
                     Arm = sdtmEntity.Arm, //Will put in characteristics for now
                     ArmCode = sdtmEntity.ArmCode,
                     DatasetId = sdtmEntity.DatasetId,
                     SubjectStartDate = sdtmEntity.RFSTDTC,
                     SubjectEndDate = sdtmEntity.RFENDTC,
                     Study = study
                 };

                 /**
                  * ADDING SUBJECT CHARACTERISTICS
                  */
                 foreach (var qualifier in sdtmEntity.Qualifiers)
                 {
                     var dsVar = dataset.Variables.SingleOrDefault(v => v.VariableDefinition.Name.Equals(qualifier.Key));
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
                                ShortName = dsVar.VariableDefinition.Name,
                                FullName = dsVar.VariableDefinition.Label,
                                Domain = "DM",
                                ProjectId = dataset.Activity.ProjectId,
                                //DataType = dsVar.VariableDefinition.DataType
                             };
                             scos.Add(dsVar.VariableDefinition.Name,sco);
                         }

                         /**
                         * ADDING SUBJECT CHARACTERISTIC
                         */
                         subject.SubjectCharacteristics.Add(new SubjectCharacteristic()
                         {
                             DatasetVariable = dsVar,
                             CharacteristicObject = sco,
                             VerbatimValue = qualifier.Value,
                             DatasetDomainCode = sdtmEntity.DomainCode,
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

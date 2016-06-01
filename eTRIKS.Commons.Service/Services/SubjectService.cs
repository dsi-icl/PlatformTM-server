using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
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
            var dataset = _datasetRepository.FindSingle(d => d.Id.Equals(datasetId),
                new List<Expression<Func<Dataset, object>>>()
                {
                    d => d.Variables.Select(v=>v.VariableDefinition),
                    d => d.Activity
                });

            //var scos = subjectData.SelectMany(s => s.Qualifiers).GroupBy(q=>q.Key).Select(g=>g.First()).Select(sco => new CharacteristicObject()
            //{
            //    Name = sco.Key,
            //    Code = sco.Key,
            //    ProjectId = dataset.Activity.ProjectId
            //    //CVtermId = , should be looked up from OLS
            //}).ToList();

            // _characteristicObjRepository.InsertMany(scos);
            var scos = new Dictionary<string,CharacteristicObject>();
            var scoList = _characteristicObjRepository.FindAll(s => s.ProjectId.Equals(dataset.Activity.ProjectId)).ToList();
            foreach (var co in scoList)
            {
                scos.Add(co.ShortName, co);
            }

             foreach (var sdtmEntity in subjectData)
             {
                 var study = _studtRepository.FindSingle(s => s.Name.Equals(sdtmEntity.VerbatimStudyId));

                 //TODO: HARD CODING ALTERT!!!
                 string site;
                 sdtmEntity.Qualifiers.TryGetValue("SITEID", out site);
                 study.Site = site;


                 /**
                  * ADDING SUBJECTS
                  */
                 var subject = new HumanSubject
                 {
                     Id = sdtmEntity.USubjId,
                     UniqueSubjectId = sdtmEntity.USubjId,
                     SubjectStudyId = sdtmEntity.SubjectId,
                     //StudyId = study.Id,//sdtmEntity.DBstudyId,//This StudyId is the STUDYID variable in the SDTM not the DB studyId 
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
                                ProjectId = dataset.Activity.ProjectId
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
                             //VerbatimName = dsVar.VariableDefinition.Label,
                             //ControlledTermStr = dsVar.VariableDefinition.Name,
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
             if (!_dataContext.Save().Equals("CREATED"))
             {
                 return false;
             }
            return true;
            
        }
    }
}

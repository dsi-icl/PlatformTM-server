﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Core.Domain.Model.DatasetModel;
using eTRIKS.Commons.Core.Domain.Model.DatasetModel.SDTM;

namespace eTRIKS.Commons.Service.Services
{
    public class BioSampleService
    {
        private readonly IRepository<Biosample, int> _bioSampleRepository;
        private readonly IRepository<Dataset, int> _datasetRepository;
        private readonly IRepository<Study, int> _studyRepository;
        private readonly IRepository<CharacteristicObject, int> _characteristicObjRepository;

        private readonly IServiceUoW _dataContext;

        public BioSampleService(IServiceUoW uoW)
        {
            _dataContext = uoW;
            _bioSampleRepository = uoW.GetRepository<Biosample, int>();
            _datasetRepository = uoW.GetRepository<Dataset, int>();
            _studyRepository = uoW.GetRepository<Study, int>();
            _characteristicObjRepository = uoW.GetRepository<CharacteristicObject, int>();
        }

        public async Task<bool> LoadBioSamples(List<SdtmRow> sampleData, int datasetId)
        {
            var dataset = _datasetRepository.FindSingle(d => d.Id.Equals(datasetId),
                new List<Expression<Func<Dataset, object>>>()
                {
                    d => d.Variables.Select(v=>v.VariableDefinition)
                });
            var studyMap = new Dictionary<string, int>();

            var scos = new Dictionary<string, CharacteristicObject>();
            var scoList = _characteristicObjRepository.FindAll(s => s.ProjectId.Equals(dataset.Activity.ProjectId)).ToList();
            foreach (var co in scoList)
            {
                scos.Add(co.ShortName, co);
            }
            foreach (SdtmRow sdtmEntity in sampleData)
            {
                Study study;
                int studyid;
                if(!studyMap.TryGetValue(sdtmEntity.StudyId, out studyid)){
                    study = _studyRepository.FindSingle(s => s.Name.Equals(sdtmEntity.StudyId));
                    studyMap.Add(sdtmEntity.StudyId,study.Id);
                    studyid = study.Id;
                }

                /**
                 * ADDING BIOSAMPLE
                 */
                var bioSample = new Biosample()
                {
                    BiosampleStudyId = sdtmEntity.SampleId,
                    AssayId = sdtmEntity.ActivityId,
                    SubjectId = sdtmEntity.USubjId,
                    StudyId = studyid,
                    //Visit = sdtmEntity.Visit,
                    CollectionStudyDay = sdtmEntity.CollectionStudyDay,
                    DatasetId = sdtmEntity.DatasetId,
                    IsBaseline = sdtmEntity.BaseLineFlag
                };


                /**
                 * ADDING SAMPLE CHARACTERISTICS
                 */
                foreach (var resqualifier in sdtmEntity.ResultQualifiers)
                {

                    var dsVar = dataset.Variables.SingleOrDefault(v => v.VariableDefinition.Name.Equals(resqualifier.Key));
                    

                    if (dsVar != null)
                    {
                        CharacteristicObject sco;
                        scos.TryGetValue(sdtmEntity.Topic, out sco);
                        if (sco == null)
                        {
                            /**
                             * CREATING NEW CHARACTERISTIC OBJECT
                             */
                            sco = new CharacteristicObject()
                            {
                                ShortName = sdtmEntity.Topic,//characteristicVar.VariableDefinition.Name,
                                FullName = sdtmEntity.TopicSynonym,//characteristicVar.VariableDefinition.Label,
                                Domain = "BS",
                                ProjectId = dataset.Activity.ProjectId,
                                //DataType = dsVar.VariableDefinition.DataType
                            };
                            scos.Add(sdtmEntity.Topic, sco);
                        }

                        bioSample.SampleCharacteristics.Add(new SampleCharacteristic()
                        {

                            DatasetVariable = dsVar,
                            CharacteristicObject = sco,
                            VerbatimName = sdtmEntity.TopicSynonym,
                            VerbatimValue = resqualifier.Value,
                            DatasetDomainCode = sdtmEntity.DomainCode
                        });
                    }
                }


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
                                Domain = "BS",
                                ProjectId = dataset.Activity.ProjectId,
                                //DataType = dsVar.VariableDefinition.DataType
                            };
                            scos.Add(dsVar.VariableDefinition.Name, sco);
                        }
                        bioSample.SampleCharacteristics.Add(new SampleCharacteristic()
                        {

                            VerbatimName = qualifier.Key,
                            VerbatimValue = qualifier.Value,
                            DatasetDomainCode = sdtmEntity.DomainCode,
                            DatasetVariable =dsVar,
                            CharacteristicObject = sco,
                        });
                    }
                }
                _bioSampleRepository.Insert(bioSample);


            }
            return _dataContext.Save().Equals("CREATED");
        }


    }
}

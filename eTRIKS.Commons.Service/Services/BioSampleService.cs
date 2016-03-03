using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Service.DTOs;
using eTRIKS.Commons.Core.Domain.Model.ControlledTerminology;

namespace eTRIKS.Commons.Service.Services
{
    public class BioSampleService
    {
        private readonly IRepository<Biosample, int> _bioSampleRepository;
        private readonly IRepository<Dataset, int> _datasetRepository;
        private readonly IRepository<Study, int> _studyRepository;

        private readonly IServiceUoW _dataContext;

        public BioSampleService(IServiceUoW uoW)
        {
            _dataContext = uoW;
            _bioSampleRepository = uoW.GetRepository<Biosample, int>();
            _datasetRepository = uoW.GetRepository<Dataset, int>();
            _studyRepository = uoW.GetRepository<Study, int>();
        }

        public async Task<bool> LoadBioSamples(List<SdtmEntity> sampleData, int datasetId)
        {
            var dataset = _datasetRepository.FindSingle(d => d.Id.Equals(datasetId),
                new List<Expression<Func<Dataset, object>>>()
                {
                    d => d.Variables.Select(v=>v.VariableDefinition)
                });
            var studyMap = new Dictionary<string, int>();
            foreach (SdtmEntity sdtmEntity in sampleData)
            {
                Study study;
                int studyid;
                if(!studyMap.TryGetValue(sdtmEntity.VerbatimStudyId, out studyid)){
                    study = _studyRepository.FindSingle(s => s.Name.Equals(sdtmEntity.VerbatimStudyId));
                    studyMap.Add(sdtmEntity.VerbatimStudyId,study.Id);
                    studyid = study.Id;
                }
                var bioSample = new Biosample()
                {
                    BiosampleStudyId = sdtmEntity.SampleId,
                    AssayId = sdtmEntity.ActivityId,
                    SubjectId = sdtmEntity.USubjId,
                    StudyId = studyid,//study.Id,//sdtmEntity.DBstudyId,
                    //Visit = sdtmEntity.Visit,
                    CollectionStudyDay = sdtmEntity.CollectionStudyDay,
                    DatasetId = sdtmEntity.DatasetId
                };
                foreach (var qualifier in sdtmEntity.Qualifiers)
                {
                    bioSample.SampleCharacteristics.Add(new SampleCharacteristic()
                    {
                       
                        VerbatimName = qualifier.Key,
                        VerbatimValue = qualifier.Value,
                        DatasetDomainCode = sdtmEntity.DomainCode,
                        DatasetVariable = dataset.Variables.SingleOrDefault(v => v.VariableDefinition.Name.Equals(qualifier.Key))
                    });
                }
                _bioSampleRepository.Insert(bioSample);


            }
            return _dataContext.Save().Equals("CREATED");
        }


    }
}

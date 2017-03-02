using System.Collections.Generic;
using System.Linq;
using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Core.Domain.Model.DatasetModel;
using eTRIKS.Commons.Core.Domain.Model.DatasetModel.SDTM;
using MongoDB.Bson;

namespace eTRIKS.Commons.Service.Services.Loading.SDTM
{
    public class BioSampleLoader
    {
        private readonly IRepository<Biosample, int> _bioSampleRepository;
        private readonly IRepository<HumanSubject, string> _subjectRepository;
        private readonly IRepository<Assay, int> _assayRepository;
        private readonly IRepository<Study, int> _studyRepository;
        private readonly IRepository<Dataset, int> _datasetRepository;

        private readonly IRepository<CharacteristicFeature, int> _characteristicObjRepository;

        private readonly IServiceUoW _dataContext;

        private Dictionary<string,CharacteristicFeature> _featureMap;

        public BioSampleLoader(IServiceUoW uoW)
        {
            _dataContext = uoW;
            _bioSampleRepository = uoW.GetRepository<Biosample, int>();
            _subjectRepository = uoW.GetRepository<HumanSubject, string>();
            _datasetRepository = uoW.GetRepository<Dataset, int>();
            _studyRepository = uoW.GetRepository<Study, int>();
            _assayRepository = uoW.GetRepository<Assay, int>();

            _characteristicObjRepository = uoW.GetRepository<CharacteristicFeature, int>();
        }

        public bool LoadBioSamples(List<SdtmRow> sampleData, SdtmSampleDescriptor descriptor, bool reload)
        {
            if (sampleData.Count == 0)
                return false;

            var projectId = sampleData.First().ProjectId;
            var projectAccession = sampleData.First().ProjectAccession;
            var assayId = sampleData.First().ActivityId;
            var datasetId = sampleData.First().DatasetId;
            var datafileId = sampleData.First().DatafileId;

            if(projectId == 0 || assayId == 0)
                return false;

            var assay = _assayRepository.Get(assayId);
            //var dataset = _datasetRepository.FindSingle(d => d.Id == datasetId);

            if (reload)
            {
                _bioSampleRepository.DeleteMany(o => o.DatasetId == datasetId && o.DataFileId == datafileId);
                _dataContext.Save().Equals("CREATED");
            }

            //TODO: CHECK if dataset is LOADED and set it to DELETE and RELOAD if so

            var studyMap = new Dictionary<string, int>();
            var featureList = _characteristicObjRepository.FindAll(s => s.ProjectId == projectId && s.ActivityId == assayId).ToList();
            _featureMap = featureList.ToDictionary(co => co.ShortName);

            var subjects = _subjectRepository.FindAll(s => s.Study.ProjectId == projectId).ToList();


            foreach (var sdtmRow in sampleData)
            {
                //RETRIEVE STUDYID FROM DB AND STORE IN MAP LOCALLY TO AVOID MAKING TOO MANY DB CALLS
                int studyid;
                if (!studyMap.TryGetValue(sdtmRow.StudyId, out studyid))
                {
                    var study = _studyRepository.FindSingle(s => s.Name == sdtmRow.StudyId && s.ProjectId == projectId);
                    studyMap.Add(sdtmRow.StudyId, study.Id);
                    studyid = study.Id;
                }


                /**
                 * ADDING BIOSAMPLE
                 */
                var bioSample = new Biosample()
                {
                    BiosampleStudyId = sdtmRow.SampleId,
                    AssayId = sdtmRow.ActivityId,
                    SubjectId = subjects.Find(s=>s.UniqueSubjectId == sdtmRow.USubjId).Id ,
                    StudyId = studyid,
                    CollectionStudyDay = sdtmRow.CollectionStudyDay,
                    DatasetId = sdtmRow.DatasetId,
                    IsBaseline = sdtmRow.BaseLineFlag
                };

                //CHECK IF ASSAY HAS TEMOPORALDATA
                var sampleDayMap = new Dictionary<string, int>();
                if (bioSample.CollectionStudyDay?.Number != null)
                {
                    if (sampleDayMap.ContainsKey(bioSample.BiosampleStudyId)
                        && sampleDayMap[bioSample.BiosampleStudyId] != bioSample.CollectionStudyDay.Number.Value
                        && !assay.HasTemporalData)
                        assay.HasTemporalData = true;
                    else
                        sampleDayMap.Add(bioSample.BiosampleStudyId, bioSample.CollectionStudyDay.Number.Value);
                }

                //ADD CHARACTERISTIC FEATURE IN BSTEST
                string charValue;
                var characValueVariable = descriptor.GetValueVariable(sdtmRow);
                if (sdtmRow.ResultQualifiers.TryGetValue(characValueVariable.Name, out charValue))
                {
                    var charFeature = getCharacteristicFeature(sdtmRow.Topic,sdtmRow.TopicSynonym,characValueVariable.DataType, projectId);

                    bioSample.SampleCharacteristics.Add(new SampleCharacteristic()
                    {
                        CharacteristicFeature = charFeature,
                        VerbatimName = sdtmRow.TopicSynonym,
                        VerbatimValue = charValue,
                        DatafileId = datafileId,
                        DatasetId = datasetId
                    });
                }

                //ADDING SPECIMEN TYPE TO CHARACTERISTICS
                string st;
                if (sdtmRow.Qualifiers.TryGetValue(descriptor.SpecimenTypeVariable.Name, out st))
                {
                    var feature = getCharacteristicFeature(descriptor.SpecimenTypeVariable.Name, descriptor.SpecimenTypeVariable.Label,
                        descriptor.SpecimenTypeVariable.DataType, projectId);

                    bioSample.SampleCharacteristics.Add(new SampleCharacteristic()
                    {
                        CharacteristicFeature = feature,
                        VerbatimName = descriptor.SpecimenTypeVariable.Name,
                        VerbatimValue = st,
                        DatafileId = datafileId,
                        DatasetId = datasetId
                    });
                }

                //ADD ANATOMIC REGION TO CHARACTERISTICS
                if (sdtmRow.QualifierQualifiers.TryGetValue(descriptor.AnatomicRegionVariable.Name, out st))
                {
                    var feature = getCharacteristicFeature(descriptor.AnatomicRegionVariable.Name, descriptor.AnatomicRegionVariable.Label,
                        descriptor.AnatomicRegionVariable.DataType, projectId);

                    bioSample.SampleCharacteristics.Add(new SampleCharacteristic()
                    {
                        CharacteristicFeature = feature,
                        VerbatimName = descriptor.AnatomicRegionVariable.Name,
                        VerbatimValue = st,
                        DatafileId = datafileId,
                        DatasetId = datasetId
                    });
                }

                _bioSampleRepository.Insert(bioSample);


            }
            return _dataContext.Save().Equals("CREATED");
        }

        private CharacteristicFeature getCharacteristicFeature(string sname, string fname, string dataType,int projectId)
        {
            // FETCH PREVIOUSLY LOADED CHARACTERISTIC FEATURE
            CharacteristicFeature feature;
            _featureMap.TryGetValue(sname, out feature);
            if (feature == null)
            {
                /**
                 * CREATING NEW CHARACTERISTIC OBJECT
                 */
                feature = new CharacteristicFeature()
                {
                    ShortName = sname,
                    FullName = fname,
                    Domain = "BS",
                    ProjectId = projectId,
                    DataType = dataType
                };
                _featureMap.Add(sname, feature);
            }
            return feature;
        }
        public void UnloadBioSamples(int datasetId)
        {
            
        }
    }
}

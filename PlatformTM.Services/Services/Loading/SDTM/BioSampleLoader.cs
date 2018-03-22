using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlatformTM.Core.Domain.Interfaces;
using PlatformTM.Core.Domain.Model;
using PlatformTM.Core.Domain.Model.DatasetModel;
using PlatformTM.Core.Domain.Model.DatasetModel.SDTM;

namespace PlatformTM.Services.Services.Loading.SDTM
{
    public class BioSampleLoader
    {
        private readonly IRepository<Biosample, int> _bioSampleRepository;
        private readonly IRepository<HumanSubject, string> _subjectRepository;
        private readonly IRepository<Assay, int> _assayRepository;
        private readonly IRepository<Study, int> _studyRepository;
        private readonly IRepository<SdtmRow, Guid> _sdtmRepository;


        private readonly IRepository<CharacteristicFeature, int> _characteristicObjRepository;

        private readonly IServiceUoW _dataContext;

        private Dictionary<string,CharacteristicFeature> _featureMap;

        public BioSampleLoader(IServiceUoW uoW)
        {
            _dataContext = uoW;
            _bioSampleRepository = uoW.GetRepository<Biosample, int>();
            _subjectRepository = uoW.GetRepository<HumanSubject, string>();
            _studyRepository = uoW.GetRepository<Study, int>();
            _assayRepository = uoW.GetRepository<Assay, int>();
            _sdtmRepository = uoW.GetRepository<SdtmRow, Guid>();


            _characteristicObjRepository = uoW.GetRepository<CharacteristicFeature, int>();
        }

        public async Task<bool> LoadBioSamples(Dataset dataset, int fileId, bool reload)
        {
            var descriptor = SdtmSampleDescriptor.GetSdtmSampleDescriptor(dataset);
            List<SdtmRow> sampleData = await _sdtmRepository.FindAllAsync(
                    dm => dm.DatasetId.Equals(dataset.Id) && dm.DatafileId.Equals(fileId));


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
            if (reload)
            {
                _bioSampleRepository.DeleteMany(o => o.DatasetId == datasetId && o.DataFileId == datafileId);
                _dataContext.Save().Equals("CREATED");
            }

            var studyMap = new Dictionary<string, int>();

            //RETRIEVE PREVIOUSLY LOADED CHARACTERISTIC FEATURES FOR THIS ASSAY THAT MIGHT HAVE BEEN LOADED BEFORE FOR OTHER STUDIES
            var featureList = _characteristicObjRepository.FindAll(s => s.ProjectId == projectId && s.ActivityId == assayId).ToList();
            _featureMap = featureList.ToDictionary(co => co.ShortName);

            var subjects = _subjectRepository.FindAll(s => s.Study.ProjectId == projectId).ToList();
            var sampleDayMap = new Dictionary<string, int>();

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
                    SubjectId = subjects.Find(s=>s.UniqueSubjectId == sdtmRow.USubjId)?.Id ,
                    StudyId = studyid,
                    CollectionStudyDay = sdtmRow.CollectionStudyDay,
                    DatasetId = sdtmRow.DatasetId,
                    DataFileId = sdtmRow.DatafileId,
                    IsBaseline = sdtmRow.Qualifiers != null && descriptor.IsBaseLineVariable != null && (sdtmRow.Qualifiers.ContainsKey(descriptor.IsBaseLineVariable?.Name) && sdtmRow.Qualifiers[descriptor.IsBaseLineVariable?.Name] == "Y")
                };

                //CHECK IF ASSAY HAS TEMOPORALDATA
               
                if (bioSample.CollectionStudyDay?.Number != null && !assay.HasTemporalData)
                {
                    
                    if (sampleDayMap.ContainsKey(bioSample.SubjectId)
                        && sampleDayMap[bioSample.SubjectId] != bioSample.CollectionStudyDay.Number.Value
                        && !assay.HasTemporalData)
                        assay.HasTemporalData = true;
                    else
                    {
                        if(!sampleDayMap.ContainsKey(bioSample.SubjectId)) 
                            sampleDayMap.Add(bioSample.SubjectId, bioSample.CollectionStudyDay.Number.Value);
                    }
                }

                //ADD CHARACTERISTIC FEATURE IN BSTEST
                string charValue;
                var characValueVariable = descriptor.GetValueVariable(sdtmRow);
                if (characValueVariable != null && sdtmRow.ResultQualifiers.TryGetValue(characValueVariable.Name, out charValue))
                {
                    var charFeature = getCharacteristicFeature(sdtmRow.Topic,sdtmRow.TopicSynonym,characValueVariable.DataType);
                    charFeature.ActivityId = assayId;
                    charFeature.ProjectId = projectId;

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
                if (descriptor.SpecimenTypeVariable != null && sdtmRow.Qualifiers.TryGetValue(descriptor.SpecimenTypeVariable.Name, out st))
                {
                    var feature = getCharacteristicFeature(descriptor.SpecimenTypeVariable.Name, descriptor.SpecimenTypeVariable.Label,
                        descriptor.SpecimenTypeVariable.DataType);
                    feature.ActivityId = assayId;
                    feature.ProjectId = projectId;

                    bioSample.SampleCharacteristics.Add(new SampleCharacteristic()
                    {
                        CharacteristicFeature = feature,
                        VerbatimName = descriptor.SpecimenTypeVariable?.Name,
                        VerbatimValue = st,
                        DatafileId = datafileId,
                        DatasetId = datasetId
                    });
                }

                //ADD ANATOMIC REGION TO CHARACTERISTICS
                if (descriptor.AnatomicRegionVariable != null && sdtmRow.QualifierQualifiers.TryGetValue(descriptor.AnatomicRegionVariable.Name, out st))
                {
                    var feature = getCharacteristicFeature(descriptor.AnatomicRegionVariable.Name, descriptor.AnatomicRegionVariable.Label,
                        descriptor.AnatomicRegionVariable.DataType);
                    feature.ActivityId = assayId;
                    feature.ProjectId = projectId;

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
                _assayRepository.Update(assay);


            }
            return _dataContext.Save().Equals("CREATED");
        }

        private CharacteristicFeature getCharacteristicFeature(string sname, string fname, string dataType)
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
                    DataType = dataType
                };
                _featureMap.Add(sname, feature);
            }
            return feature;
        }
        public bool UnloadBioSamples(int datasetId, int fileId)
        {
            _bioSampleRepository.DeleteMany(s => s.DatasetId == datasetId && s.DataFileId.Value == fileId);
            _sdtmRepository.DeleteMany(s => s.DatafileId == fileId && s.DatasetId == datasetId);

            return _dataContext.Save() == "CREATED";
        }
    }
}

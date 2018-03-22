using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlatformTM.Core.Domain.Interfaces;
using PlatformTM.Core.Domain.Model;
using PlatformTM.Core.Domain.Model.DatasetModel;
using PlatformTM.Core.Domain.Model.DatasetModel.SDTM;
using PlatformTM.Core.Domain.Model.DesignElements;
using PlatformTM.Core.JoinEntities;

namespace PlatformTM.Services.Services.Loading.SDTM
{
    public class SubjectLoader

    {
        private readonly IRepository<HumanSubject, string> _subjectRepository;
        private readonly IRepository<Study, int> _studtRepository;
        private readonly IRepository<CharacteristicFeature, int> _characteristicObjRepository;
        private readonly IRepository<SdtmRow, Guid> _sdtmRepository;

        private readonly IServiceUoW _dataContext;

        public SubjectLoader(IServiceUoW uoW)
        {
            _dataContext = uoW;
            _subjectRepository = uoW.GetRepository<HumanSubject, string>();
            _studtRepository = uoW.GetRepository<Study, int>();
            _characteristicObjRepository = uoW.GetRepository<CharacteristicFeature, int>();
            _sdtmRepository = uoW.GetRepository<SdtmRow, Guid>();

        }

        
        public async Task<bool> LoadSubjects(Dataset dataset, int fileId, bool reload)
        {
            var descriptor = SdtmSubjectDescriptor.GetSdtmSubjectDescriptor(dataset);
            List<SdtmRow> sdtmData = await _sdtmRepository.FindAllAsync(
                    dm => dm.DatasetId.Equals(dataset.Id) && dm.DatafileId.Equals(fileId));

            if (sdtmData.Count == 0)
                return false;

            var projectId = sdtmData.First().ProjectId;
            var projectAccession = sdtmData.First().ProjectAccession;
            var activityId = sdtmData.First().ActivityId;
            var datafileId = sdtmData.First().DatafileId;

            if (reload)
            {
                _subjectRepository.DeleteMany(o => o.DatasetId == dataset.Id && o.DatafileId == datafileId);
                _dataContext.Save().Equals("CREATED");
            }

            //Project related subject characteristics
            var scoList = _characteristicObjRepository.FindAll(s => s.ProjectId == projectId).ToList();
            var scos = scoList.ToDictionary(co => co.ShortName);
            //Project related studies
            var studies = _studtRepository.FindAll(s => s.ProjectId == projectId, new List<string> { "Arms.Arm" }).ToList();
            var studyMap = studies.ToDictionary(study => study.Name);
            //Project related arms
            var arms = studies.SelectMany(s => s.Arms.Select(a => a.Arm)).Distinct();
            var armMap = arms.ToDictionary(arm => arm.Name);

            //Previously loaded subjects for this project that might be update by this load
            //TODO: should I add datafileId to subject as well? to be able to delete subjects for this datafile
            var subjects = _subjectRepository.FindAll(s => s.Study.ProjectId == projectId).ToList();

            foreach (var sdtmSubject in sdtmData)
            {
                Study study;
                if (!studyMap.TryGetValue(sdtmSubject.StudyId, out study))
                {
                    study = new Study()
                    {
                        Name = sdtmSubject.StudyId,
                        Accession = "S-" + projectAccession.Substring(2) + "-" + (studies.Count() + 1).ToString("00"),
                        ProjectId = projectId
                    }; studyMap.Add(study.Name, study);
                }
                Arm arm;
                if (!armMap.TryGetValue(sdtmSubject.QualifierSynonyms[descriptor.ArmVariable.Name], out arm))
                {
                    arm = new Arm()
                    {
                        Id = "P-"+ projectId + "-ARM-" + sdtmSubject.Qualifiers[descriptor.ArmCodeVariable.Name],
                        Code = sdtmSubject.Qualifiers[descriptor.ArmCodeVariable.Name],
                        Name = sdtmSubject.QualifierSynonyms[descriptor.ArmVariable.Name]
                    };
                    armMap.Add(arm.Name, arm);
                }
                if (!arm.Studies.Exists(s => s.Study.Name == study.Name))
                    arm.Studies.Add(new StudyArm() { Arm = arm, Study = study });


                var subjNewFlag = false;
                /**
                  * ADDING SUBJECTS
                  */
                var subject = subjects.Find(s => s.UniqueSubjectId == sdtmSubject.USubjId);
                if (subject == null)
                {
                    subject = new HumanSubject
                    {
                        Id = "P-" + projectId + "-" + sdtmSubject.USubjId,
                        UniqueSubjectId = sdtmSubject.USubjId,
                        SubjectStudyId = sdtmSubject.SubjectId,
                        Arm = sdtmSubject.QualifierSynonyms[descriptor.ArmVariable.Name], //Will put in characteristics for now
                        ArmCode = sdtmSubject.Qualifiers[descriptor.ArmCodeVariable.Name],
                        DatasetId = sdtmSubject.DatasetId,
                        DatafileId = sdtmSubject.DatafileId,
                        Study = study,
                        StudyArm = arm
                    };
                    subjNewFlag = true;
                }
                else
                {
                    subjNewFlag = false;
                    subject.DatasetId = dataset.Id;
                    subject.DatafileId = sdtmSubject.DatafileId;
                    subject.Arm = sdtmSubject.QualifierSynonyms[descriptor.ArmVariable.Name];
                    subject.ArmCode = sdtmSubject.Qualifiers[descriptor.ArmCodeVariable.Name];
                    subject.Study = study;
                    subject.StudyArm = arm;
                }

                //SET/UPDATE SUBJECT CHARACTERISTICS

                if (descriptor.RefStartDate != null && sdtmSubject.Qualifiers.ContainsKey(descriptor?.RefStartDate?.Name))
                {
                    DateTime startDate;
                    if (DateTime.TryParse(sdtmSubject.Qualifiers[descriptor.RefStartDate?.Name], out startDate))
                        subject.SubjectStartDate = startDate;
                }

                if (descriptor.RefEndDate != null && sdtmSubject.Qualifiers.ContainsKey(descriptor?.RefEndDate?.Name))
                {
                    DateTime endDate;
                    if (DateTime.TryParse(sdtmSubject.Qualifiers[descriptor.RefEndDate.Name], out endDate))
                        subject.SubjectEndDate = endDate;
                }

                //THIS METHOD WILL DELETE ALL SUBJECT CHARACTERISTICS PREVIOUSLY LOADED 
                //FOR THIS SUBJECT FOR THE SAME DATAFILE
                //((List<SubjectCharacteristic>)subject.SubjectCharacteristics)
                //    .RemoveAll(sc=>sc.DatafileId == datafileId && sc.DatasetId == datasetId);
                //_subjectRepository.Update(subject);
                //_dataContext.Save();

                /**
                 * ADDING SUBJECT CHARACTERISTICS
                 */
                foreach (var qualifier in sdtmSubject.Qualifiers)
                {
                    //THIS CHECKS THAT ONLY COLUMNS IN DATA THAT ARE PRESENT IN THE TEMPLATE ARE PARSED
                    var dsVar = descriptor.QualifierVariables.SingleOrDefault(v => v.Name.Equals(qualifier.Key));

                    if (descriptor.CharacteristicProperties.Exists(c=>c.Name == qualifier.Key) ||
                        descriptor.TimingVariableDefinitions.Exists(c => c.Name == qualifier.Key))
                    {
                        CharacteristicFeature sco;
                        scos.TryGetValue(qualifier.Key, out sco);
                        if (sco == null)
                        {
                            /**
                             * CREATING NEW CHARACTERISTIC OBJECT
                             */
                            sco = new CharacteristicFeature()
                            {
                                ShortName = dsVar.Name,
                                FullName = dsVar.Label,
                                Domain = "DM",
                                ProjectId = projectId,
                                DataType = dsVar.DataType,
                                ActivityId = activityId
                            };
                            scos.Add(dsVar.Name, sco);
                        }

                        /**
                        * ADDING SUBJECT CHARACTERISTIC
                        */
                        subject.SubjectCharacteristics.Add(new SubjectCharacteristic()
                        {
                            CharacteristicFeature = sco,
                            VerbatimValue = qualifier.Value,
                            VerbatimName = sco.ShortName,
                            Subject = subject,
                            DatasetId = dataset.Id,
                            DatafileId = datafileId
                        });
                    }
                    
                }
                if (subjNewFlag)
                    _subjectRepository.Insert(subject);
                else _subjectRepository.Update(subject);
            }
            return _dataContext.Save().Equals("CREATED");
        }

        public bool UnloadSubjects(int datasetId, int fileId)
        {

            _subjectRepository.DeleteMany(s=>s.DatasetId == datasetId && s.DatafileId == fileId);
            _sdtmRepository.DeleteMany(s => s.DatafileId == fileId && s.DatasetId == datasetId);

            return _dataContext.Save() == "CREATED";
        }
    }
}

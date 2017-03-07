﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Core.Domain.Model.DatasetModel.SDTM;
using eTRIKS.Commons.Core.Domain.Model.DesignElements;
using eTRIKS.Commons.Core.JoinEntities;

namespace eTRIKS.Commons.Service.Services.Loading.SDTM
{
    public class SubjectLoader

    {
        private readonly IRepository<HumanSubject, string> _subjectRepository;
        private readonly IRepository<Study, int> _studtRepository;
        private readonly IRepository<CharacteristicFeature, int> _characteristicObjRepository;

        private readonly IServiceUoW _dataContext;

        public SubjectLoader(IServiceUoW uoW)
        {
            _dataContext = uoW;
            _subjectRepository = uoW.GetRepository<HumanSubject, string>();
            _studtRepository = uoW.GetRepository<Study, int>();
            _characteristicObjRepository = uoW.GetRepository<CharacteristicFeature, int>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subjectData"></param>
        /// <param name="descriptor"></param>
        /// <returns></returns>
        public bool LoadSubjects(List<SdtmRow> subjectData, SdtmSubjectDescriptor descriptor, bool reload)
        {
            if (subjectData.Count == 0)
                return false;

            var projectId = subjectData.First().ProjectId;
            var projectAccession = subjectData.First().ProjectAccession;
            var activityId = subjectData.First().ActivityId;
            var datafileId = subjectData.First().DatafileId;
            var datasetId = subjectData.First().DatasetId;

            if (reload)
            {
                _subjectRepository.DeleteMany(o => o.DatasetId == datasetId && o.DatafileId == datafileId);
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

            foreach (var sdtmSubject in subjectData)
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
                        Id = projectAccession + "-" + sdtmSubject.Qualifiers[descriptor.ArmCodeVariable.Name],
                        Code = sdtmSubject.Qualifiers[descriptor.ArmCodeVariable.Name],
                        Name = sdtmSubject.QualifierSynonyms[descriptor.ArmVariable.Name]
                    };
                    if (arm.Studies == null) arm.Studies = new List<StudyArm>();
                    if (!arm.Studies.Exists(s => s.Study.Name == study.Name)) arm.Studies.Add(new StudyArm() { Arm = arm, Study = study });
                    armMap.Add(arm.Name, arm);
                }

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
                    subject.DatasetId = datasetId;
                }

                //SET/UPDATE SUBJECT CHARACTERISTICS

                if (descriptor.RefStartDate != null && sdtmSubject.Qualifiers[descriptor?.RefStartDate?.Name] != null)
                {
                    DateTime startDate;
                    if (DateTime.TryParse(sdtmSubject.Qualifiers[descriptor.RefStartDate?.Name], out startDate))
                        subject.SubjectStartDate = startDate;
                }

                if (descriptor.RefEndDate != null && sdtmSubject.Qualifiers[descriptor.RefEndDate?.Name] != null)
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
                    if (dsVar != null)
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
                            DatasetId = datasetId,
                            DatafileId = datafileId
                        });
                    }
                    else
                        throw new Exception("Qualifier not in dataset template");
                }
                if (subjNewFlag)
                    _subjectRepository.Insert(subject);
                else _subjectRepository.Update(subject);
            }
            return _dataContext.Save().Equals("CREATED");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using PlatformTM.Core.Domain.Interfaces;
using PlatformTM.Core.Domain.Model;
using PlatformTM.Core.Domain.Model.DatasetModel.PDS;
using PlatformTM.Services.DTOs;

namespace PlatformTM.Services.Services
{
    public class AssessmentService
    {
        private readonly IServiceUoW _dataContext;
        private readonly IRepository<Assessment, int> _assessmentRepository;
        private readonly IRepository<PrimaryDataset, int> _pdsRepository;
        public AssessmentService(IServiceUoW uoW)
        {
            _dataContext = uoW;
            _assessmentRepository = uoW.GetRepository<Assessment, int>();
            _pdsRepository = uoW.GetRepository<PrimaryDataset, int>();

        }

        public AssessmentDTO GetAssessmentForStudy(int studyId, int assessmentId)
        {

            Assessment assessment = (assessmentId == 0)
                ? new Assessment() { StudyId = studyId }
                : _assessmentRepository
                .FindSingle(d => d.Id == assessmentId, new List<string>(){ "Datasets"});

            var studyDatasets = _pdsRepository.FindAll(d => d.Studies.Any(s=>s.Id == studyId)).ToList();
            //m => m.Roles.Any(r => roles.Contains(r.Name))

            var assessmentDTO = WriteToDTO(assessment,studyDatasets);

            return assessmentDTO;
        }

        public List<AssessmentDTO> GetStudyAssessments(int studyId)
        {
            List<Assessment> assessments = _assessmentRepository
               .FindAll(d => d.StudyId == studyId, new List<string>() { "Datasets" }).ToList();

            var studyDatasets = _pdsRepository.FindAll(d => d.Studies.Any(s => s.Id == studyId)).ToList();

            return assessments.Select(s => WriteToDTO(s, studyDatasets)).ToList();
        }

        public Assessment AddStudyAssessment(AssessmentDTO assessmentDTO)
        {
            var newAssessment = ReadFromDTO(assessmentDTO, new Assessment() { StudyId = assessmentDTO.StudyId });
            newAssessment = _assessmentRepository.Insert(newAssessment);
            return (_dataContext.Save().Equals("CREATED")) ? newAssessment : null;

        }

        public Assessment UpdateStudyAssessment(AssessmentDTO assessmentDTO, int assessmentId)
        {
            var assessment = _assessmentRepository
                .FindSingle(d => d.Id == assessmentId, new List<string>() { "Datasets" });

            assessment = ReadFromDTO(assessmentDTO, assessment);
            _assessmentRepository.Update(assessment);
            return _dataContext.Save().Equals("CREATED") ? assessment : null ;
        }

        private Assessment ReadFromDTO(AssessmentDTO dto, Assessment assessment)
        {
            assessment.Name = dto.Name;
            assessment.Status = dto.Status;
            assessment.Description = dto.Description;

            if (dto.AssociatedDatasets != null && dto.AssociatedDatasets.Count != 0)
            {
                var selectedDSids = dto.AssociatedDatasets.Where(a => a.IsSelected).ToList().Select(a => a.Id);
                //var unselectedDSids = dto.AssociatedDatasets.Where(a => !a.IsSelected).ToList().Select(a => a.Id);

                var selectedDatasets = _pdsRepository.FindAll(s => selectedDSids.Contains(s.Id));
                if (selectedDatasets == null)
                    return null;

                if(assessment.Datasets.Count != 0)
                    ((List<PrimaryDataset>)assessment.Datasets).Clear();

                ((List<PrimaryDataset>)assessment.Datasets).AddRange(selectedDatasets.ToList());
            }
            return assessment;
        }


        private AssessmentDTO WriteToDTO(Assessment assessment, List<PrimaryDataset> studyDatasets)
        {
            AssessmentDTO assessmentDTO = null;
            if (assessment != null)
            {
                assessmentDTO = new AssessmentDTO()
                {
                    Id = assessment.Id,
                    Name = assessment.Name,
                    Description = assessment.Description,
                    Status = assessment.Status,
                    StudyId = assessment.StudyId

                };

                foreach (var ds in studyDatasets)
                {
                    assessmentDTO.AssociatedDatasets.Add(new AssessmentDatasetDTO()
                    {
                        Title = ds.Title,
                        Description = ds.Description,
                        Domain = ds.Domain,
                        Id = ds.Id,
                        IsSelected = ((List<PrimaryDataset>)assessment.Datasets).Exists(d => d.Id == ds.Id)
                    });
                }
            }
            return assessmentDTO;
        }
    }
}


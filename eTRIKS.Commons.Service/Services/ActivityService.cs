/************************************************************/
/********   Created By: Dilshan Silva 12-12-2014   **********/
/******** Services to handle functions on Activity **********/
/************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Service.DTOs;
using System.Linq.Expressions;
using eTRIKS.Commons.DataAccess.MongoDB;


namespace eTRIKS.Commons.Service.Services
{
    public class ActivityService
    {
        private IRepository<Activity, int> _activityRepository;
        private IRepository<Dataset, int> _dataSetRepository;
        private IRepository<VariableDefinition, int> _variableDefinition;
        private IRepository<Assay, int> _assayRepository;
        private IRepository<Project, int> _projectRepository;
        private IServiceUoW _activityServiceUnit;
        private DatasetService _datasetService;


        public ActivityService(IServiceUoW uoW, DatasetService datasetService)
        {
            _activityServiceUnit = uoW;
            _activityRepository = uoW.GetRepository<Activity, int>();
            _dataSetRepository = uoW.GetRepository<Dataset, int>();
            _projectRepository = uoW.GetRepository<Project, int>();
            _variableDefinition =  uoW.GetRepository<VariableDefinition, int>();
            _datasetService = datasetService;
        }

        private static readonly Expression<Func<Activity, ActivityDTO>> AsBookDto =
        x => new ActivityDTO
        {
            Id = x.Id,
            Name = x.Name,
            ProjectId = x.ProjectId,
            datasets = x.Datasets.Select(m => new DatasetDTO
            {
                Name = m.Domain.Name,
                Id = m.Id
            }).ToList()
        };

        public ActivityDTO addActivity(ActivityDTO activityDTO)
        {
            var project = _projectRepository.FindSingle(d=>d.Accession
                .Equals(activityDTO.ProjectAcc));
            var activity = new Activity {Name = activityDTO.Name, ProjectId = project.Id};
            foreach (var datasetDto in activityDTO.datasets)
            {
                datasetDto.ProjectId = project.Id;
                var dataset = _datasetService.CreateDataset(datasetDto);
                activity.Datasets.Add(dataset);
            }
            

            _activityRepository.Insert(activity);
            if (_activityServiceUnit.Save().Equals("CREATED"))
            {
                activityDTO.Id = activity.Id;
                return activityDTO;
            }
            return null;
        }

        public bool checkExist(int activityId)
        {
            Activity activity = new Activity();
            activity =_activityRepository.Get(activityId);
            if (activity == null)
            {
                return false;
            }
            return true;
        }


        public ActivityDTO getActivityDTOById(int activityId)
        {
            Activity activity = _activityRepository.FindSingle(
                d => d.Id.Equals(activityId),
                new List<Expression<Func<Activity, object>>>(){
                        d => d.Datasets.Select(t => t.Domain)
                }
            );

            ActivityDTO activityDTO = new ActivityDTO();
            activityDTO.Name = activity.Name;
            activityDTO.Id = activity.Id;
            activityDTO.ProjectId = activity.ProjectId;
            if (activity.GetType() == typeof (Assay))
                activityDTO.isAssay = true;

            foreach (var ds in activity.Datasets)
            {
                DatasetDTO dst = _datasetService.GetActivityDatasetDTO(ds.Id);
                activityDTO.datasets.Add(dst);
            }
            return activityDTO;
        }
        public IEnumerable<ActivityDTO> getStudyActivities(string projectAccession)
        {
            IEnumerable<Activity> activities;

            // Typed lambda expression for Select() method. 
            activities = _activityRepository.FindAll(
                    d => d.Project.Accession.Equals(projectAccession),
                    new List<Expression<Func<Activity, object>>>(){
                        d => d.Datasets.Select(t => t.Domain),
                        d => d.Project
                    }
                );
            return activities.Select(p => new ActivityDTO
            {
                Name = p.Name,
                Id = p.Id,
                ProjectId = p.ProjectId,
                ProjectAcc = p.Project.Accession,
                datasets = p.Datasets.Select(m => new DatasetDTO
                {
                    Name = m.Domain.Name,
                    Id = m.Id,
                    DomainId = m.DomainId
                }).ToList()
            }).ToList();
        }

        public string updateActivity(ActivityDTO activityDTO, int activityId)
        {
            Activity activityToUpdate = _activityRepository.Get(activityId);

            activityToUpdate.Name = activityDTO.Name;
            foreach (var datasetDto in activityDTO.datasets)
            {
                //datasetDto.ProjectId = project.Id;
                if (datasetDto.isNew)
                {
                    var dataset = _datasetService.CreateDataset(datasetDto);
                    activityToUpdate.Datasets.Add(dataset);
                }
                else
                    _datasetService.updateDataset(datasetDto);                
            }
            return _activityServiceUnit.Save();
        }
    }
}

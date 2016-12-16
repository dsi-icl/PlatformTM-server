/************************************************************/
/********   Created By: Dilshan Silva 12-12-2014   **********/
/******** Services to handle functions on Activity **********/
/************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Service.DTOs;
using System.Linq.Expressions;
using eTRIKS.Commons.Core.Domain.Model.DatasetModel;


namespace eTRIKS.Commons.Service.Services
{
    public class ActivityService
    {
        private readonly IRepository<Activity, int> _activityRepository;
        private readonly IServiceUoW _activityServiceUnit;
        private readonly DatasetService _datasetService;


        public ActivityService(IServiceUoW uoW, DatasetService datasetService)
        {
            _activityServiceUnit = uoW;
            _activityRepository = uoW.GetRepository<Activity, int>();
            _datasetService = datasetService;
        }

        public ActivityDTO GetActivity(int activityId)
        {
            Activity activity = _activityRepository.FindSingle(
                d => d.Id.Equals(activityId),
                new List<string>(){
                        "Datasets.Domain"
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

        public ActivityDTO AddActivity(ActivityDTO activityDTO)
        {
            //var project = _projectRepository.FindSingle(d => d.Accession
            //    .Equals(activityDTO.ProjectAcc));
            var activity = new Activity { Name = activityDTO.Name, ProjectId = activityDTO.ProjectId};
            foreach (var datasetDto in activityDTO.datasets)
            {
                datasetDto.ProjectId = activityDTO.ProjectId;
                var dataset = _datasetService.CreateDataset(datasetDto);
                activity.Datasets.Add(dataset);
            }


            activity = _activityRepository.Insert(activity);
            if (_activityServiceUnit.Save().Equals("CREATED"))
            {
                activityDTO.Id = activity.Id;
                return activityDTO;
            }
            return null;
        }
     
        public string UpdateActivity(ActivityDTO activityDTO, int activityId)
        {
            Activity activityToUpdate = _activityRepository.Get(activityId);

            activityToUpdate.Name = activityDTO.Name;
            foreach (var datasetDto in activityDTO.datasets)
            {
                if (datasetDto.isNew)
                {
                    datasetDto.ProjectId = activityDTO.ProjectId;
                    var dataset = _datasetService.CreateDataset(datasetDto);
                    
                    activityToUpdate.Datasets.Add(dataset);
                    _activityRepository.Update(activityToUpdate);
                }
                else
                    _datasetService.UpdateDataset(datasetDto);                
            }
            return _activityServiceUnit.Save();
        }

        public void DeleteActivity(int activity)
        {

        }

        

       
    }
}

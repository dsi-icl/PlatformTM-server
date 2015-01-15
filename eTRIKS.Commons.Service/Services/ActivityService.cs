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

namespace eTRIKS.Commons.Service.Services
{
    public class ActivityService
    {
        private IRepository<Activity, int> _activityRepository;
        private IRepository<Dataset, int> _dataSetRepository;
        private IServiceUoW _activityServiceUnit;

        public ActivityService(IServiceUoW uoW)
        {
            _activityServiceUnit = uoW;
            _activityRepository = uoW.GetRepository<Activity, int>();
            _dataSetRepository = uoW.GetRepository<Dataset, int>();
        }

        private static readonly Expression<Func<Activity, ActivityDTO>> AsBookDto =
        x => new ActivityDTO
        {
            Name = x.Name,
            StudyID = x.StudyId,
            datasets = x.Datasets.Select(m => new DatasetBriefDTO
            {
                Name = m.Domain.Name,
                Id = m.OID
            }).ToList()
        };

        public Activity addActivity(ActivityDTO activityDTO)
        {
            Activity activity = new Activity();
            activity.Name = activityDTO.Name;
            activity.StudyId = activityDTO.StudyID;

            _activityRepository.Insert(activity);
            if (_activityServiceUnit.Save().Equals("CREATED")){
                return activity;
            }
            return null;
        }

        public bool checkExist(int activityId)
        {
            Activity activity = new Activity();
            activity =_activityRepository.GetById(activityId);
            if (activity == null)
            {
                return false;
            }
            return true;
        }


        public ActivityDTO getActivityDTOById(int activityId)
        {
            Activity activity = _activityRepository.GetSingle(
                d => d.OID.Equals(activityId),
                new List<Expression<Func<Activity, object>>>(){
                        d => d.Datasets.Select(t => t.Domain)
                }
            );

            ActivityDTO activityDTO = new ActivityDTO();
            activityDTO.Name = activity.Name;
            activityDTO.Id = activity.OID;
            activityDTO.StudyID = activity.StudyId;
            Dataset ds = activity.Datasets.FirstOrDefault();
            if (ds != null){
                DatasetBriefDTO dst = new DatasetBriefDTO();
                dst.Name = ds.Domain.Name;
                dst.Id = ds.OID; 
            activityDTO.datasets = new List<DatasetBriefDTO>{dst};
            }
            
            return activityDTO;
        }

        public IEnumerable<ActivityDTO> getStudyActivities(string studyId)
        {
            IEnumerable<Activity> activities;

            // Typed lambda expression for Select() method. 
            activities = _activityRepository.Get(
                    d => d.StudyId.Equals(studyId),
                    new List<Expression<Func<Activity, object>>>(){
                        d => d.Datasets.Select(t => t.Domain)
                    }
                );
            return activities.Select(p => new ActivityDTO
            {
                Name = p.Name,
                Id = p.OID,
                StudyID = p.StudyId,
                datasets = p.Datasets.Select(m => new DatasetBriefDTO
                {
                    Name = m.Domain.Name,
                    Id = m.OID
                }).ToList()
            }).ToList();
        }

        public string updateActivity(ActivityDTO activityDTO, int activityId)
        {
            Activity activityToUpdate = _activityRepository.GetById(activityId);

            activityToUpdate.Name = activityDTO.Name;
            return _activityServiceUnit.Save();
        }
    }
}

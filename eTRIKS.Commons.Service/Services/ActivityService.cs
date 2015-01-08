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

namespace eTRIKS.Commons.Service.Services
{
    public class ActivityService
    {
        private IRepository<Activity, int> _activityRepository;
        private IRepository<Dataset, string> _dataSetRepository;
        private IServiceUoW _activityServiceUnit;

        public ActivityService(IServiceUoW uoW)
        {
            _activityServiceUnit = uoW;
            _activityRepository = uoW.GetRepository<Activity, int>();
            _dataSetRepository = uoW.GetRepository<Dataset, string>();
        }

        public void addActivity(ActivityDTO activityDTO)
        {
            Activity activity = new Activity();
            //activity.OID = "ACT-TST-02";//activityDTO.OID;
            activity.Name = activityDTO.Name;
            activity.StudyId = activityDTO.StudyID;


            _activityRepository.Insert(activity);
            _activityServiceUnit.Save();
        }

        public void addDataset(Dataset dataset)
        {
            _dataSetRepository.Insert(dataset);
            _activityServiceUnit.Save();
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

        public Activity getActivityById(int activityId)
        {
            return _activityRepository.GetById(activityId);
        }

        public Activity getActivity(string studyId, string activityId)
        {
            return _activityRepository.GetList(o => o.OID.Equals(activityId) && o.StudyId.Equals(studyId), d=> d.Datasets);
        }

        public IEnumerable<Activity> getStudyActivities(string studyId)
        {
            return _activityRepository.GetRecords(o => o.StudyId.Equals(studyId));
        }
    }
}

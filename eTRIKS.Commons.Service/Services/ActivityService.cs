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

namespace eTRIKS.Commons.Service.Services
{
    public class ActivityService
    {
        private IRepository<Activity, string> _activityRepository;
        private IRepository<Dataset, string> _dataSetRepository;
        private IServiceUoW _activityServiceUnit;

        public ActivityService(IServiceUoW uoW)
        {
            _activityServiceUnit = uoW;
            _activityRepository = uoW.GetRepository<Activity, string>();
            _dataSetRepository = uoW.GetRepository<Dataset, string>();
        }

        public void addActivity(Activity activity)
        {
            _activityRepository.Insert(activity);
            _activityServiceUnit.Save();
        }

        public void addDataset(Dataset dataset)
        {
            _dataSetRepository.Insert(dataset);
            _activityServiceUnit.Save();
        }

        public bool checkExist(string activityId)
        {
            Activity activity = new Activity();
            activity =_activityRepository.GetById(activityId);
            if (activity == null)
            {
                return false;
            }
            return true;
        }

        public Activity getActivityById(string activityId)
        {
            return _activityRepository.GetById(activityId);
        }

        public Activity getActivity(string studyId, string activityId)
        {
            return _activityRepository.GetRecord(o => o.OID.Equals(activityId) && o.StudyId.Equals(studyId), d=> d.Datasets);
        }

        public IEnumerable<Activity> getStudyActivities(string studyId)
        {
            return _activityRepository.GetRecords(o => o.StudyId.Equals(studyId));
        }
    }
}

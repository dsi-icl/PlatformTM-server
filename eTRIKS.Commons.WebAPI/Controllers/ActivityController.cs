/************************************************************/
/********   Created By: Dilshan Silva 12-12-2014   **********/
/******** Services to handle functions on Activity **********/
/************************************************************/

using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Service.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using eTRIKS.Commons.Service.DTOs;

namespace eTRIKS.Commons.WebAPI.Controllers
{
    //[RoutePrefix("api/activities")]
    public class ActivityController : ApiController
    {
        private ActivityService _activityService;

        public ActivityController(ActivityService activityService)
        {
            _activityService = activityService;
        }

        [HttpPost]
        [Route("api/activities")]
        public HttpResponseMessage addActivity([FromBody] ActivityDTO activityDTO)
        {
            Activity activity = new Activity();
            activity.OID = "ACT-TST-01";//activityDTO.OID;
            activity.Name = activityDTO.Name;
            activity.StudyId = activityDTO.StudyID;

            _activityService.addActivity(activity);
            return Request.CreateResponse(HttpStatusCode.Created);
        }

        //[HttpPost]
        //public HttpResponseMessage AddDataSet(string activityOID, [FromBody] DatasetDTO datasetDTO)
        //{
        //    if (_activityService.checkExist(activityOID))
        //    {
        //        Dataset dataset = new Dataset();
        //        dataset.OID = datasetDTO.OID;
        //        dataset.ActivityId = datasetDTO.Activity;
        //        dataset.DomainId = datasetDTO.Domain;
        //        _activityService.addDataset(dataset);
        //        return Request.CreateResponse(HttpStatusCode.Created);
        //    }
        //    return Request.CreateResponse(HttpStatusCode.NotFound);
        //}

        [HttpGet]
        [Route("api/activities/{activityId}")]
        public Activity GetActivityByKey(string activityId)
        {
            return _activityService.getActivityById(activityId);
        }

        [HttpGet]
        [Route("api/studies/{studyId}/activities/{activityId}")]
        public Activity getActivity(string studyId, string activityId)
        {
            return _activityService.getActivity(studyId, activityId);
        }

        [HttpGet]
        [Route("api/studies/{studyId}/activities")]
        public IEnumerable<Activity> getStudyActivities(string studyId)
        {
            return _activityService.getStudyActivities(studyId);
        }



    }
}

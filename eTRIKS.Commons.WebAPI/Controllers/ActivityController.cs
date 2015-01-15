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
using System.Web.Http.Cors;

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

        [HttpGet]
        [Route("api/activities/{activityId}", Name="GetActivityById")]
        public ActivityDTO getActivity(int activityId)
        {
            return _activityService.getActivityDTOById(activityId);
        }

        [HttpGet]
        [Route("api/studies/{studyId}/activities")]
        public IEnumerable<ActivityDTO> getStudyActivities(string studyId)
        {
            return _activityService.getStudyActivities(studyId);
        }

        [HttpPost]
        [Route("api/activities")]
        public HttpResponseMessage addActivity([FromBody] ActivityDTO activityDTO)
        {
            var addedActivity = _activityService.addActivity(activityDTO);

            if (addedActivity != null)
            {
                var response = Request.CreateResponse<Activity>(HttpStatusCode.Created, addedActivity);
                string uri = Url.Link("GetActivityById", new { activityId = addedActivity.OID });
                response.Headers.Location = new Uri(uri);
                return response;
            }
            else
            {
                var response = Request.CreateResponse(HttpStatusCode.Conflict);
                return response;
            }
        }

        [HttpPut]
        [Route("api/activities/{activityId}")]
        public HttpResponseMessage updateActivity(int activityId, [FromBody] ActivityDTO activityDTO)
        {
            try{
                _activityService.updateActivity(activityDTO, activityId);
                var response = Request.CreateResponse<ActivityDTO>(HttpStatusCode.Accepted, activityDTO);
                string uri = Url.Link("GetActivityById", new { activityId = activityDTO.Id });
                response.Headers.Location = new Uri(uri);
                return response;
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.Conflict);
            }
        }
    }
}

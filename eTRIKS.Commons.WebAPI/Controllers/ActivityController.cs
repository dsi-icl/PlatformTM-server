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
        private AssayService _assayService;

        public ActivityController(ActivityService activityService, AssayService assayService)
        {
            _activityService = activityService;
            _assayService = assayService;
        }

        [HttpGet]
        [Route("api/activities/{activityId}", Name="GetActivityById")]
        public ActivityDTO getActivity(int activityId)
        {
            return _activityService.GetActivity(activityId);
        }

        [HttpPost]
        [Route("api/activities")]
        public HttpResponseMessage addActivity([FromBody] ActivityDTO activityDTO)
        {
            ActivityDTO addedActivity =null;
            if(!activityDTO.isAssay)
                addedActivity = _activityService.AddActivity(activityDTO);
            //if (activityDTO.isAssay)
                //addedActivity = _assayService.AddAssay(activityDTO);

            if (addedActivity != null)
            {
                var response = Request.CreateResponse<ActivityDTO>(HttpStatusCode.Created, addedActivity);
                string uri = Url.Link("GetActivityById", new { activityId = addedActivity.Id });
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
                _activityService.UpdateActivity(activityDTO, activityId);
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

        [HttpGet]
        [Route("api/studies/{studyId}/activities")]
        public IEnumerable<ActivityDTO> getStudyActivities(string studyId)
        {
            return _activityService.GetStudyActivities(studyId);
        }
        /**
         * Assay Methods
         * 
         * */

        [HttpGet]
        [Route("api/assays/{assayId}", Name = "GetAssayById")]
        public AssayDTO getAssay(int assayId)
        {
            return _activityService.GetAssay(assayId);
        }

        [HttpPost]
        [Route("api/assays")]
        public HttpResponseMessage addAssay([FromBody] AssayDTO assayDTO)
        {
            AssayDTO addedAssay = null;

            addedAssay = _activityService.AddAssay(assayDTO);

            if (addedAssay != null)
            {
                var response = Request.CreateResponse<AssayDTO>(HttpStatusCode.Created, addedAssay);
                string uri = Url.Link("GetAssayById", new { assayId = addedAssay.Id });
                response.Headers.Location = new Uri(uri);
                return response;
            }
            else
            {
                var response = Request.CreateResponse(HttpStatusCode.Conflict);
                return response;
            }
        }
    }
}

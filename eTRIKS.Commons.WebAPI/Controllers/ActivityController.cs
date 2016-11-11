using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Service.Services;
using System;
using System.Net;
using System.Net.Http;
using eTRIKS.Commons.Service.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace eTRIKS.Commons.WebAPI.Controllers
{
    public class ActivityController : Controller
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

        [HttpPut]
        [Route("api/assays/{assayId}")]
        public HttpResponseMessage updateAssay(int assayId, [FromBody] AssayDTO assayDTO)
        {
            try
            {
                _activityService.UpdateAssay(assayDTO, assayId);
                var response = Request.CreateResponse<AssayDTO>(HttpStatusCode.Accepted, assayDTO);
                string uri = Url.Link("GetAssayById", new { assayId = assayDTO.Id });
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

using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Service.Services;
using System;
using eTRIKS.Commons.Service.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

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
        public IActionResult addActivity([FromBody] ActivityDTO activityDTO)
        {
            ActivityDTO addedActivity =null;
            if(!activityDTO.isAssay)
                addedActivity = _activityService.AddActivity(activityDTO);
            //if (activityDTO.isAssay)
                //addedActivity = _assayService.AddAssay(activityDTO);

            if (addedActivity != null)
            {
                //var response = Request.CreateResponse<ActivityDTO>(HttpStatusCode.Created, addedActivity);
                //string uri = Url.Link("GetActivityById", new { activityId = addedActivity.Id });
                //response.Headers.Location = new Uri(uri);
                //return response;
                return new CreatedAtActionResult("GET", "GetActivityById", new { activityId = addedActivity.Id }, addedActivity);

            }
            else
            {
                return new StatusCodeResult(StatusCodes.Status409Conflict);
            }
        }

        [HttpPut]
        [Route("api/activities/{activityId}")]
        public IActionResult updateActivity(int activityId, [FromBody] ActivityDTO activityDTO)
        {
            try{
                _activityService.UpdateActivity(activityDTO, activityId);
                //var response = Request.CreateResponse<ActivityDTO>(HttpStatusCode.Accepted, activityDTO);
                //string uri = Url.Link("GetActivityById", new { activityId = activityDTO.Id });
                //response.Headers.Location = new Uri(uri);
                //return response;
                return new CreatedAtActionResult("GET", "GetActivityById", new { activityId = activityDTO.Id }, activityDTO);
            }
            catch (Exception e)
            {
                return new StatusCodeResult(StatusCodes.Status409Conflict);
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
        public IActionResult addAssay([FromBody] AssayDTO assayDTO)
        {
            AssayDTO addedAssay = null;

            addedAssay = _activityService.AddAssay(assayDTO);

            if (addedAssay != null)
            {
                //var response = Request.CreateResponse<AssayDTO>(HttpStatusCode.Created, addedAssay);
                //string uri = Url.Link("GetAssayById", new { assayId = addedAssay.Id });
                //response.Headers.Location = new Uri(uri);
                return new CreatedAtActionResult("GET", "GetAssayById", new { assayId = addedAssay.Id }, addedAssay);

            }
            else
            {
                //var response = Request.CreateResponse(HttpStatusCode.Conflict);
                //return response;
                return new StatusCodeResult(StatusCodes.Status409Conflict);
            }
        }

        [HttpPut]
        [Route("api/assays/{assayId}")]
        public IActionResult updateAssay(int assayId, [FromBody] AssayDTO assayDTO)
        {
            try
            {
                _activityService.UpdateAssay(assayDTO, assayId);
                //var response = Request.CreateResponse<AssayDTO>(HttpStatusCode.Accepted, assayDTO);
                //string uri = Url.Link("GetAssayById", new { assayId = assayDTO.Id });
                //response.Headers.Location = new Uri(uri);
                //return response;
                return new CreatedAtActionResult("GET", "GetAssayById", new { assayId = assayDTO.Id }, assayDTO);

            }
            catch (Exception e)
            {
                return new StatusCodeResult(StatusCodes.Status409Conflict);
            }
        }

 
    }
}

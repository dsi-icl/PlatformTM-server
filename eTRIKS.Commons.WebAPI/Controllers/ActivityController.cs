using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Service.Services;
using System;
using eTRIKS.Commons.Service.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace eTRIKS.Commons.WebAPI.Controllers
{
    [Route("api/activities")]
    public class ActivityController : Controller
    {
        private ActivityService _activityService;
        private AssayService _assayService;

        public ActivityController(ActivityService activityService, AssayService assayService)
        {
            _activityService = activityService;
            _assayService = assayService;
        }

        [HttpGet("{activityId}", Name = "GetActivityById")]
        public ActivityDTO getActivity(int activityId)
        {
            return _activityService.GetActivity(activityId);
        }

        [HttpPost]
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

        [HttpPut("{activityId}")]
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

        
       
    }
}

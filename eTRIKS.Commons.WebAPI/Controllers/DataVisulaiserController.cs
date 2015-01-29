using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Service.DTOs;
using eTRIKS.Commons.Service.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace eTRIKS.Commons.WebAPI.Controllers
{
    public class DataVisulaiserController : ApiController
    {
        private ActivityService _activityService;

        public DataVisulaiserController(ActivityService activityService)
        {
            _activityService = activityService;
        }
        

        [HttpGet]
        [Route("api/DataVisulaiser/{studyId}")]
        public IEnumerable<Activity> getActivityData(string studyId)
        {
            return _activityService.getActivityData(studyId);
        }
    }
}

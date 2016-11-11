using System;
using System.Net;
using System.Net.Http;
using eTRIKS.Commons.Service.DTOs;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Service.Services;
using Microsoft.AspNetCore.Mvc;

namespace eTRIKS.Commons.WebAPI.Controllers
{
    public class StudyController : Controller
    {
         private StudyService _studyService;

         public StudyController(StudyService studyService)
        {
            _studyService = studyService;
        }

        [HttpGet]
        [Route("api/studies/{studyId}", Name = "GetstudyById")]
        public StudyDTO Getstudy(int studyId)
        {
            return _studyService.GetstudyId(studyId);
        }


        [HttpPost]
        [Route("api/studies")]
        public HttpResponseMessage Addstudy([FromBody] StudyDTO studyDTO)
        {
            StudyDTO addedstudy = null;

            addedstudy = _studyService.Addstudy(studyDTO);

            if (addedstudy != null)
            {
                var response = Request.CreateResponse<StudyDTO>(HttpStatusCode.Created, addedstudy);
                string uri = Url.Link("GetstudyById", new { studyId = addedstudy.Id });
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
        [Route("api/studies/{studyId}")]
        public HttpResponseMessage Updatestudy(int studyId, [FromBody] StudyDTO studyDTO)
        {
            try
            {
                _studyService.Updatestudy(studyDTO, studyId);
                var response = Request.CreateResponse<StudyDTO>(HttpStatusCode.Accepted, studyDTO);
                string uri = Url.Link("GetstudyById", new { studyId = studyDTO.Id });
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

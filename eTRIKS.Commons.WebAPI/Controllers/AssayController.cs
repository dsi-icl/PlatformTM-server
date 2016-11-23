using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using eTRIKS.Commons.Service.DTOs;
using eTRIKS.Commons.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace eTRIKS.Commons.WebAPI.Controllers
{
    [Authorize]
    [Route("api/assays")]
    public class AssayController : Controller
    {
        private readonly AssayService _assayService;

        public AssayController(AssayService assayService)
        {
            _assayService = assayService;
        }

        [HttpGet("{assayId}", Name = "GetAssayById")]
        public AssayDTO getAssay(int assayId)
        {
            return _assayService.GetAssay(assayId);
        }

        [HttpPost]
        public IActionResult addAssay([FromBody] AssayDTO assayDTO)
        {
            AssayDTO addedAssay = null;

            addedAssay = _assayService.AddAssay(assayDTO);

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

        [HttpPut("{assayId}")]
        public IActionResult updateAssay(int assayId, [FromBody] AssayDTO assayDTO)
        {
            try
            {
                _assayService.UpdateAssay(assayDTO, assayId);
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


        [HttpGet("{assayId}/samples")]
        public Hashtable GetAssaySamples(int assayId)
        {
            return _assayService.GetSamplesDataPerAssay(assayId);
        }
        //[HttpGet]
        //[Route("api/assays/temp/{assayId}/loadPA/{fileId}")]
        //public IActionResult AddPA(int assayId, int fileId)
        //{
        //    _assayService.addPA(assayId, fileId);
        //    return new OkResult();
        //}
    }
}

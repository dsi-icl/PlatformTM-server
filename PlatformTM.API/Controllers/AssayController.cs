using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlatformTM.Models.DTOs;
using PlatformTM.Models.Services;

namespace PlatformTM.API.Controllers
{
    [Route("assays")]
    public class AssayController : Controller
    {
        private readonly AssayService _assayService;

        public AssayController(AssayService assayService)
        {
            _assayService = assayService;
        }

        [HttpGet("{assayId}", Name = "GetAssayById")]
        public AssayDTO GetAssay(int assayId)
        {
            return _assayService.GetAssay(assayId);
        }

        [HttpPost]
        public IActionResult AddAssay([FromBody] AssayDTO assayDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var addedAssay = _assayService.AddAssay(assayDTO);
            if (addedAssay != null)
                return new CreatedAtRouteResult("GetAssayById", new { assayId = addedAssay.Id }, addedAssay);
            return new StatusCodeResult(StatusCodes.Status409Conflict);
        }

        [HttpPut("{assayId}")]
        public IActionResult UpdateAssay(int assayId, [FromBody] AssayDTO assayDTO)
        {
            try
            {
                _assayService.UpdateAssay(assayDTO, assayId);
                return new AcceptedAtRouteResult("GetAssayById", new { assayId = assayDTO.Id }, assayDTO);
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }
        }
    }
}

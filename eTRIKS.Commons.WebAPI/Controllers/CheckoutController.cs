using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eTRIKS.Commons.Service.DTOs;
using eTRIKS.Commons.Service.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace eTRIKS.Commons.WebAPI.Controllers
{
    [Route("checkout")]
    public class CheckoutController : Controller
    {
        private readonly CheckoutService _checkoutService;
        public CheckoutController(CheckoutService checkoutService)
        {
            _checkoutService = checkoutService;
        }

        [HttpGet("{cartId}")]
        public IActionResult CheckoutDatasets(string cartId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var datasets = _checkoutService.CreateCheckoutDatasets(cartId,userId);
            if (datasets != null)
                return Ok(datasets);
            return NotFound();
        }
        // GET: api/values
        [HttpGet]
        public IEnumerable<string> GetCheckoutDatasets()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet("datasets/{datasetId}/preview")]
        public DataTable GetDataPreview(string datasetId)
        {
            
            return _checkoutService.ExportDataset(datasetId);//.(projectId, userDatasetDto);
            
            // return _checkoutService.ExportDataset(datasetId);//.(projectId, userDatasetDto);
        }


        [Route("datasets/{datasetId}/download")]
        [HttpGet]
       // public void DownloadDatasets(DataTable dtTable)
        public void DownloadDatasets(string datasetId)
        {

            var dtTable = _checkoutService.ExportDataset(datasetId);
            HttpContext.Response.Clear();
            HttpContext.Response.ContentType = "text/csv";
            HttpContext.Response.Headers.Add("content-disposition", "attachment;filename=data.csv");
            // HttpContext.Response.WriteAsync(_checkoutService.downloadDatasets(dtTable));
            HttpContext.Response.WriteAsync(_checkoutService.downloadDatasets(dtTable));
            //HttpContext.Abort();
        }





        /*
        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
        */
    }
}

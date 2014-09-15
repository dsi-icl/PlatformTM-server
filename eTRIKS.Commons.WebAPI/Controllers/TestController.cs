
using System.Collections.Generic;
using System.Web.Http;
using eTRIKS.Commons.Core.Application.Services;
using eTRIKS.Commons.Core.Domain.Model.Templates;

namespace eTRIKS.Commons.WebAPI.Controllers
{
    public class TestController : ApiController
    {
       //TODO: Not a generic Interface for Service, but Interface for a concerete definition of TemplatesService

        private ITemplateService _templateService;
        // GET: api/Test

        public TestController(ITemplateService templateService)
        {
            _templateService = templateService;
        }

        public IEnumerable<DomainTemplate> Get()
        {
            return _templateService.GetAllDomains();
            //return new string[] { "value1", "value2" };
        }

        // GET: api/Test/5
        public DomainTemplate Get(string id)
        {
            return _templateService.GetDomainTemplateById(id);
        }

        // POST: api/Test
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Test/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Test/5
        public void Delete(int id)
        {
        }
    }
}

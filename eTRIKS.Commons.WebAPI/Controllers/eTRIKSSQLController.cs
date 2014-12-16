using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Data.Entity;
using eTRIKS.Commons.Core.Domain.Model.Templates;
using eTRIKS.Commons.Core.Application.Services;
using System.Linq.Expressions;



namespace eTRIKS.Commons.WebAPI.Controllers
{
    [RoutePrefix("api/etriksSQL")]

    public class eTRIKSSQLController<T> : ApiController
    {
        private ITemplateService _templateService;

        [Route("{id}")]
        public DomainTemplate Get(string id)
        {

            return _templateService.GetDomainTemplateById(id);
        }



    }
}


using eTRIKS.Commons.Service.Services.OntologyManagement;

namespace eTRIKS.Commons.WebAPI.Controllers
{
    public class OntologyController
    {
        private readonly IOntologyService _OLSservice;

        public OntologyController(IOntologyService ontologyService)
        {
            _OLSservice = ontologyService;
        }
    }
}

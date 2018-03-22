using PlatformTM.Services.Services.OntologyManagement;

namespace PlatformTM.API.Controllers
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

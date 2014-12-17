using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using eTRIKS.Commons.Core.Domain.Model.Templates;

using eTRIKS.Commons.Service.Services;
using eTRIKS.Commons.Service.DTOs;

using System.Web.Http.Cors;
using eTRIKS.Commons.Core.Domain.Model;

namespace eTRIKS.Commons.WebAPI.Controllers
{
    [EnableCors(origins: "http://localhost:63342", headers: "*", methods: "GET,POST")]
    public class DatasetController : ApiController
    {
        private DatasetService _datasetService;

        public DatasetController(DatasetService datasetService)
        {
            _datasetService = datasetService;
        }
        
        // GET: api/Dataset
        //[EnableCors(origins: "http://localhost:63342", headers: "*", methods: "*")]
        [HttpGet]
        [Route("api/Dataset")]
        public IEnumerable<DomainTemplate> Get()
        {
            //List<DomainTemplate> ts = new List<DomainTemplate>();
            //DomainTemplate dt = new DomainTemplate();
            //dt.Class = "test1";
            //ts.Add(dt);
            //dt = new DomainTemplate();
            //dt.Class = "test2";
            //ts.Add(dt);
            //return ts;
            return _datasetService.GetAllDomainTemplates();
        }

       

        // GET: api/Dataset/5
        //[EnableCors(origins: "http://localhost:63342", headers: "*", methods: "*")]
        [HttpGet]
        [Route("api/Dataset/{domainId}")]
        public DatasetDTO Get(string domainId)
        {
            return _datasetService.GetTemplateDataset(domainId);
        }

        //[HttpPost]
        //public void Add([FromBody] List<VariableReferenceDTO> varRefDTOList)
        //{
        //    List<VariableReference> varRefList = new List<VariableReference>();
        //    for (int i = 0; i < varRefDTOList.Count; i++)
        //    {
        //        VariableReference varRef = new VariableReference();
        //        varRef.VariableDefinitionId = varRefDTOList[i].VariableDefinitionId;
        //        varRef.DatasetId = varRefDTOList[i].DatasetId;
        //        //varRef.OID = varRefDTOList[i].VariableDefinitionId;
        //        // Continue for the rest of the fields
        //        varRefList.Add(varRef);
        //    }
        //    _datasetService.addDatasetVariableReferences(varRefList);
        //}


        [HttpPost]
        public void addDataset([FromBody] DatasetDTO datasetDTO)
        {
            Dataset dataset = new Dataset();
            List<VariableDefinition> varDefList = new List<VariableDefinition>();
            List<VariableReference> varRefList = new List<VariableReference>();
            for (int i = 0; i < datasetDTO.variables.Count; i++)
            {
                if (datasetDTO.variables[i].isSelected)
                {
                    //1. add to the varRefList and varDef
                }
            }
            //2. Load the vardef's
            DomainTemplate templates = new DomainTemplate();
            templates = _datasetService.getTemplateDomainVariables(datasetDTO.DomainId);
            //3. Load the dataset
            dataset.Variables = varRefList;
            _datasetService.addDataset(dataset);

        }

        //public DomainTemplate GetDomain(string id)
        //{
        //    return _datasetService.GetTemplateDatasetNew(id);
        //}

        // DELETE: api/Dataset/5
        public void Delete(int id)
        {
        }
    }
}

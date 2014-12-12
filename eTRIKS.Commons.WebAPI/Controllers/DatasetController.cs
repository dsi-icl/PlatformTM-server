using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using eTRIKS.Commons.Core.Domain.Model.Templates;
using eTRIKS.Commons.Service.DTOs;
using eTRIKS.Commons.Service.Services;
using eTRIKS.Commons.Core.Domain.Model;


namespace eTRIKS.Commons.WebAPI.Controllers
{
    public class DatasetController : ApiController
    {
        private DatasetService _datasetService;

        public DatasetController(DatasetService datasetService)
        {
            _datasetService = datasetService;
        }

        [HttpPost]
        public void Add([FromBody] List<VariableReferenceDTO> varRefDTOList)
        {
            List<VariableReference> varRefList = new List<VariableReference>();
            for (int i = 0; i < varRefDTOList.Count; i++)
            {
                VariableReference varRef = new VariableReference();
                varRef.VariableDefinitionId = varRefDTOList[i].VariableDefinitionId;
                varRef.DatasetId = varRefDTOList[i].DatasetId;
                varRef.OID = varRefDTOList[i].VariableDefinitionId;
                // Continue for the rest of the fields
                varRefList.Add(varRef);
            }
            _datasetService.addDatasetReferences(varRefList);
        }

        //public DomainDataset GetDomain(string id)
        //{
        //    return _datasetService.GetTemplateDatasetNew(id);
        //}

    }
}

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
        [Route("api/Dataset")]
        public string addDataset([FromBody] DatasetDTO datasetDTO)
        {
            // create an OID for dataset
            // DAT-UBP-01
            //string lastOID = _datasetService.getDataSetOID("DAT-UBP");

            //1. Fields for Dataset
            Dataset dataset = new Dataset();
            //dataset.OID = "DAT-UBP-0T";
            dataset.ActivityId = datasetDTO.ActivityId;
            dataset.DomainId = datasetDTO.DomainId;

            List<VariableDefinition> varDefList = new List<VariableDefinition>();
            List<VariableReference> varRefList = new List<VariableReference>();
            for (int i = 0; i < datasetDTO.variables.Count; i++)
            {
                if (datasetDTO.variables[i].isSelected)
                {
                    //2. Fields for varDef
                    VariableDefinition varDef = new VariableDefinition();
                    varDef.OID = datasetDTO.variables[i].Id;
                    varDef.Name= datasetDTO.variables[i].Name;
                    varDef.Label = datasetDTO.variables[i].Label;
                    varDef.Description = datasetDTO.variables[i].Description;
                    varDef.DataType = datasetDTO.variables[i].DataType;
                    varDef.StudyId = datasetDTO.variables[i].StudyId;
                    varDefList.Add(varDef);

                    //3. Fields for varRefList
                    VariableReference varRef = new VariableReference();
                    varRef.DatasetId = dataset.OID;
                    varRef.VariableDefinitionId = datasetDTO.variables[i].Id;
                    varRef.OrderNumber = datasetDTO.variables[i].OrderNumber;
                    varRef.IsRequired = datasetDTO.variables[i].IsRequired;
                    varRefList.Add(varRef);
                }
            }
            //4. Load the VarDef and 5. Load the dataset & VarRef
            dataset.Variables = varRefList;
            return _datasetService.addDataset(dataset, varDefList);
        }


        [HttpPost]
        [Route("api/Dataset")]
        public string updateDataset(string studyId, [FromBody] DatasetDTO datasetDTO)
        {
            Dataset dataset = new Dataset();
            dataset.OID = "DAT-UBP-0T";
            //dataset.ActivityId = "ACT-UBP-01";
            dataset.DomainId = datasetDTO.DomainId;

            List<VariableDefinition> variableDefsOfStudy = _datasetService.getVariableDefinitionsOfStudy(studyId).ToList();
            List<VariableReference> variableRefsOfActivity = _datasetService.GetActivityDataset(dataset.OID).Variables.ToList();

            List<VariableDefinition> varDefList = new List<VariableDefinition>();
            List<VariableReference> varRefList = new List<VariableReference>();
            for (int i = 0; i < datasetDTO.variables.Count; i++)
            {
                if (datasetDTO.variables[i].isSelected)
                {
                    if (!variableDefsOfStudy.Exists(d => d.OID.Equals(datasetDTO.variables[i].Id)))
                    {
                        //2. Fields for varDef
                        VariableDefinition varDef = new VariableDefinition();
                        varDef.OID = datasetDTO.variables[i].Id;
                        varDef.Name = datasetDTO.variables[i].Name;
                        varDef.Label = datasetDTO.variables[i].Label;
                        varDef.Description = datasetDTO.variables[i].Description;
                        varDef.DataType = datasetDTO.variables[i].DataType;
                        varDef.StudyId = studyId;
                        varDefList.Add(varDef);
                    }
                    if (!variableRefsOfActivity.Exists(d => d.VariableDefinitionId.Equals(datasetDTO.variables[i].Id)))
                    {
                        //3. Fields for varRefList
                        VariableReference varRef = new VariableReference();
                        varRef.DatasetId = dataset.OID;
                        varRef.VariableDefinitionId = datasetDTO.variables[i].Id;
                        varRef.OrderNumber = datasetDTO.variables[i].OrderNumber;
                        varRef.IsRequired = datasetDTO.variables[i].IsRequired;
                        varRefList.Add(varRef);
                    }
                }
            }
            //4. Load the VarDef and 5. Load the dataset & VarRef
            dataset.Variables = varRefList;
            return _datasetService.updateDataset(dataset, varDefList);
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

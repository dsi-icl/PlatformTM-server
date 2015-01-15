/************************************************************/
/********   Created By: Dilshan Silva 12-12-2014   **********/
/********  Services to handle functions on a Study **********/
/************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using eTRIKS.Commons.Service.DTOs;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Service.Services;

namespace eTRIKS.Commons.WebAPI.Controllers
{
    public class StudyController : ApiController
    {
         private StudyService _studyService;

         public StudyController(StudyService studyService)
        {
            _studyService = studyService;
        }

        /*
        [HttpPost]
        public void AddDatasetVariables([FromBody] List<VariableDefinitionDTO> varDefDTOList)
        {
            List <VariableDefinition> varDefList = new List<VariableDefinition>();
            for (int i = 0; i < varDefDTOList.Count; i++)
            {
                VariableDefinition varDef = new VariableDefinition();
                varDef.Accession = varDefDTOList[i].;
                varDef.Name = varDefDTOList[i].Name;
                varDef.StudyId = varDefDTOList[i].StudyId;
                // Continue for the rest of the fields
                varDefList.Add(varDef);
            }
            _studyService.addDatasetVariables(varDefList);
        }*/

    }
}

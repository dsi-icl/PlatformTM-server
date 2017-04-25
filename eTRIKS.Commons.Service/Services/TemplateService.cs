using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.Core.Domain.Model.Templates;
using eTRIKS.Commons.Core.Domain.Model.ControlledTerminology;
using eTRIKS.Commons.DataAccess.Repositories;
using eTRIKS.Commons.Service.DTOs;

namespace eTRIKS.Commons.Service.Services
{
    public class TemplateService
    {

        private readonly IServiceUoW _dataServiceUnit;
        private readonly IRepository<DatasetTemplate, string> _templateRepository;

        public TemplateService(IServiceUoW uoW)
        {
            _dataServiceUnit = uoW;
            _templateRepository = uoW.GetRepository<DatasetTemplate, string>();
        }


        /// <summary>
        /// Retrieves Domain dataset from Template Tables for "New" datasets
        /// </summary>
        /// <param name="domainId"></param>
        public DatasetDTO GetTemplateDataset(string domainId)
        {
            var datasetTemplate = _templateRepository.FindAll(
                d => d.Id.Equals(domainId),
                new List<string>()
                {
                   "Fields.ControlledVocabulary.Xref.DB",
                   "Fields.QualifiersDictionary.Terms"
                }
                ).FirstOrDefault();


            //TODO: USE AutoMapper instead of this manual mapping

            if (datasetTemplate == null)
                return null;

            var dto = GetDatasetDTO(datasetTemplate);

            return dto;
        }

        public List<DatasetDTO> GetAllDomainTemplates()
        {
            var domains = _templateRepository.FindAll(
                d => d.Id.Contains("D-SDTM"), 
                new List<string>()
                {
                    "Fields"
                }).ToList()
                  .OrderBy(d => d.Class);
            return domains.Select(GetDatasetDTO).ToList();
        }

        public List<DatasetDTO> GetAssayFeatureTemplates()
        {
            var templates = _templateRepository.FindAll(
                d => d.Class == "Assay Features", new List<string>() { "Fields" });
            return templates.Select(GetDatasetDTO).ToList();
        }

        public List<DatasetDTO> GetAssaySampleTemplates()
        {
            var templates = _templateRepository.FindAll(
                d => d.Class == "Assay Samples", new List<string>() {"Fields" });
            return templates.Select(GetDatasetDTO).ToList();
        }

        public List<DatasetDTO> GetAssayDataTemplates()
        {
            var templates = _templateRepository.FindAll(
                d => d.Class == "Assay Observations", new List<string>(){"Fields"});
            return templates.Select(GetDatasetDTO).ToList();
        }

        private DatasetDTO GetDatasetDTO(DatasetTemplate datasetTemplate)
        {
            var dto = new DatasetDTO
            {
                Class = datasetTemplate.Class,
                Description = datasetTemplate.Description,
                Name = datasetTemplate.Domain,
                DomainId = datasetTemplate.Id,
                Structure = datasetTemplate.Structure,
                Code = datasetTemplate.Code,
                HasHeader = datasetTemplate.Fields.Any(f => f.Section == "HEADER")
            };

            foreach (var vt in datasetTemplate.Fields.OrderBy(c => c.Order))
            {
                var dv = new DatasetVariableDTO
                {
                    Name = vt.Name,
                    Description = vt.Description,
                    Label = vt.Label,
                    Accession = vt.Id,
                    RoleId = vt.RoleId,
                    DataType = vt.DataType,
                    UsageId = vt.UsageId,
                    IsRequired = false,
                    KeySequence = null,
                    OrderNumber = null,
                    IsCurated = true,
                    IsComputed = false,
                    varType = "STANDARD",
                    IsGeneric = vt.IsGeneric,
                    AllowedQualifiers = vt.QualifiersDictionary?.Terms?.Select(t => t.Name).ToList(),
                    DictionaryName = vt.ControlledVocabulary?.Name,
                    DictionaryXrefURL = vt.ControlledVocabulary?.Xref.DB.UrlPrefix + vt.ControlledVocabulary?.Xref.Accession 
                };
               

                if (dv.UsageId.Equals("CL-Compliance-T-1") || dv.UsageId.Equals("CL-Compliance-T-2"))
                {
                    dv.isSelected = true;
                    if (dv.UsageId.Equals("CL-Compliance-T-1"))
                        dv.IsRequired = true;
                }

                if(vt.Section == "HEADER")
                    dto.HeaderFields.Add(dv);
                if(vt.IsGeneric)
                    dto.GenericFields.Add(dv);
                else if(vt.Section == "COLUMN")
                    dto.Variables.Add(dv);
            }
            return dto;

        }


    }
}

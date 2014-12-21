using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Core.Domain.Model.Templates;
using eTRIKS.Commons.Service.DTOs;

namespace eTRIKS.Commons.Service.Services
{
    public class DatasetService
    {
        private IServiceUoW _dataServiceUnit;
        private IRepository<Dataset, string> _datasetRepository;
        private IRepository<DomainTemplate, string> _domainRepository;
        private IRepository<Activity, string> _activityRepository;
        private IRepository<VariableDefinition, string> _variableDefinitionRepository;

        public DatasetService(IServiceUoW uoW)
        {
            _dataServiceUnit = uoW;
            //_templateRepository = uoW.GetRepository<DomainTemplate, string>();
            //_templateVariableRepository = uoW.GetRepository<DomainTemplateVariable,string>();
            _datasetRepository = uoW.GetRepository<Dataset, string>();
            _domainRepository = uoW.GetRepository<DomainTemplate, string>();
            _variableDefinitionRepository = uoW.GetRepository<VariableDefinition, string>();
        }

        /// <summary>
        /// Retrieves Domain dataset from Template Tables for "New" datasets
        /// </summary>
        /// <param name="domainId"></param>
        public DatasetDTO GetTemplateDataset(string domainId)
        {
            DomainTemplate domainTemplate = _domainRepository.Get(
                d => d.OID.Equals(domainId),
                new List<Expression<Func<DomainTemplate, object>>>(){
                    d => d.Variables.Select(c => c.controlledTerminology.Xref.DB)
                },
                null,
                null
                ).FirstOrDefault<DomainTemplate>();


            //TODO: USE AutoMapper instead of this manual mapping

            DatasetDTO dto = new DatasetDTO();

            dto.Class = domainTemplate.Class;
            dto.Description = domainTemplate.Description;
            dto.Name = domainTemplate.Name;
            dto.DomainId = domainTemplate.OID;
            dto.Structure = domainTemplate.Structure;
            List<DatasetVariableDTO> vars = new List<DatasetVariableDTO>();
            foreach (DomainVariableTemplate vt in domainTemplate.Variables)
            {
                DatasetVariableDTO dv = new DatasetVariableDTO();
                dv.Name = vt.Name;
                dv.Description = vt.Description;
                dv.Label = vt.Label;
                dv.Id = vt.OID;
                if (vt.controlledTerminology != null)
                {
                    dv.DictionaryName = vt.controlledTerminology.Name;
                    dv.DictionaryDefinition = vt.controlledTerminology.Definition;
                    dv.DictionaryXrefURL = vt.controlledTerminology.Xref.DB.UrlPrefix+vt.controlledTerminology.Xref.Accession;
                }
                dv.IsRequired = null;
                dv.KeySequence = null;
                dv.OrderNumber = null;
                    
                vars.Add(dv);
            }
            dto.variables = vars;

            return dto;
        }


        public List<DomainTemplate> GetAllDomainTemplates()
        {
            return _domainRepository.GetAllList();
        }

        

        /// <summary>
        /// Retrieves dataset for selected activity, which includes Variable_Defs, DomainVariables and Variable_Refs
        /// </summary>
        /// <param name="activityId"></param>
        public Dataset GetActivityDataset(string datasetId)
        {
            //getActivityDataset including var_refs and var_defs
            return _datasetRepository.GetList(o => o.OID.Equals(datasetId), d => d.Variables);
            
        }

        public string addDataset(Dataset dataset, List<VariableDefinition> variableDefinitions)
        {
            _datasetRepository.Insert(dataset);

            for (int i = 0; i < variableDefinitions.Count; i++)
            {
                _variableDefinitionRepository.Insert(variableDefinitions[i]);
            }
            return _dataServiceUnit.Save();
        }

        public string updateDataset(Dataset dataset, List<VariableDefinition> variableDefinitions)
        {
            _datasetRepository.Update(dataset);

            for (int i = 0; i < variableDefinitions.Count; i++)
            {
                _variableDefinitionRepository.Insert(variableDefinitions[i]);
            }
            return _dataServiceUnit.Save();
        }


        public DomainTemplate getTemplateDomainVariables(string domainId)
        {
            return _domainRepository.GetList(o => o.OID.Equals(domainId), d=>d.Variables);
        }

        public string getDataSetOID(string OIDPrefix)
        {
            var latestDataset = _datasetRepository.Get(o => o.OID.Contains(OIDPrefix), null, d => d.OrderByDescending(o => o.OID), null, null).First();
            return latestDataset.OID;
        }

        public IEnumerable<VariableDefinition> getVariableDefinitionsOfStudy(string studyId)
        {
           return _variableDefinitionRepository.GetRecords(d => d.StudyId.Equals(studyId));
        }


    }
}

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
        private IRepository<VariableReference, string> _variableReferenceRepository; 

        public DatasetService(IServiceUoW uoW)
        {
            _dataServiceUnit = uoW;
            //_templateRepository = uoW.GetRepository<DomainTemplate, string>();
            //_templateVariableRepository = uoW.GetRepository<DomainTemplateVariable,string>();
            _datasetRepository = uoW.GetRepository<Dataset, string>();
            _domainRepository = uoW.GetRepository<DomainTemplate, string>();
            _variableReferenceRepository = uoW.GetRepository<VariableReference, string>();
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
                    d => d.Variables.Select(c => c.controlledTerminology.Xref)
                },
                null,
                null
                ).FirstOrDefault<DomainTemplate>();


            //TODO: USE AutoMapper instead of this manual mapping

            DatasetDTO dto = new DatasetDTO();

            dto.Class = domainTemplate.Class;
            dto.Description = domainTemplate.Description;
            dto.Name = domainTemplate.Name;
            List<DatasetVariableDTO> vars = new List<DatasetVariableDTO>();
            foreach (DomainVariableTemplate vt in domainTemplate.Variables)
            {
                DatasetVariableDTO dv = new DatasetVariableDTO();
                dv.Name = vt.Name;
                dv.Description = vt.Description;
                dv.Label = vt.Label;

                if (vt.controlledTerminology != null)
                {
                    dv.DictionaryName = vt.controlledTerminology.Name;
                    dv.DictionaryDefinition = vt.controlledTerminology.Definition;
                    dv.DictionaryXrefURL = vt.controlledTerminology.Xref.Accession;
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
        public void GetActivityDataset(string activityId)
        {
            //getActivityDataset including var_refs and var_defs

            //create datasetDTO which includes domain metadata, a list of variables defs adding var_ref attributes to them
            //getDomainVariables where domain_id = dataset.domain_id
            //add variables which are not in the var_def collection
        }

        public void addDataset()
        {
            //For a new dataset 
            //input: datasetDTO
            //Extract from datasetDTO the variables
            //new dataset
            //Iterate over datasetDTO variables
            //for each variable create new variable_Ref (use createOrRetrieve) add to datasets var_def collection
            //
        }

        public void addDatasetVariableReferences(List<VariableReference> variableReferences)
        {
            for (int i = 0; i < variableReferences.Count; i++)
            {
                _variableReferenceRepository.Insert(variableReferences[i]);
            }
            _dataServiceUnit.Save();
        }


    }
}

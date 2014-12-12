using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Core.Domain.Model.Templates;

namespace eTRIKS.Commons.Service.Services
{
    public class DatasetService
    {
        private IServiceUoW _dataServiceUnit;
        private IRepository<Dataset, string> _datasetRepository;
        private IRepository<DomainDataset, string> _domainRepository; 

        public DatasetService(IServiceUoW uoW)
        {
            _dataServiceUnit = uoW;
            //_templateRepository = uoW.GetRepository<DomainTemplate, string>();
            //_templateVariableRepository = uoW.GetRepository<DomainTemplateVariable,string>();
            _datasetRepository = uoW.GetRepository<Dataset, string>();
            _domainRepository = uoW.GetRepository<DomainDataset, string>();
        }

        /// <summary>
        /// Retrieves Domain dataset from Template Tables for "New" datasets
        /// </summary>
        /// <param name="domainId"></param>
        public DomainDataset GetTemplateDataset(string domainId)
        {
            DomainDataset domainTemplate = _domainRepository.Get(
                d => d.OID == domainId,
                null,
                new List<Expression<Func<DomainDataset, object>>>()
                {
                    d => d.Variables
                        
                }).FirstOrDefault();

            return domainTemplate;
        }


        public DomainDataset GetTemplateDatasetNew(string domainId)
        {
            //IList<Templates_DomainDataset_TBL> tempDomVar3 = chkExist.GetList(o => o.OID.Equals("D-SDTM-LB"), d => d.Templates_DomainVariable_TBL.Select(t => t.Dictionary_TBL));

            //IList<DomainDataset> domainTemplate = _domainRepository.GetList(d => d.OID.Equals(domainId));
            
            ///DomainDataset domainTemplate = _domainRepository.GetList(d => d.OID.Equals(domainId), d => d.Variables);
            //DomainDataset domainTemplate = _domainRepository.GetList(d => d.OID.Equals(domainId), d => d.Variables.Select(t => t.controlledTerminology));
            DomainDataset domainTemplate = _domainRepository.GetList(d => d.OID.Equals(domainId));
            _dataServiceUnit.Dispose();

            return domainTemplate;
        }

        /// <summary>
        /// Retrives dataset for selected activity, which includes Variable_Defs, DomainVariables and Variable_Refs
        /// </summary>
        /// <param name="datasetId"></param>
        public void GetActivityDataset(string datasetId)
        {
            //getActivityDataset including var_refs and var_defs

            //create datasetDTO which includes domain metadata, a list of variables defs adding var_ref attributes to them
            //getDomainVariables where domain_id = dataset.domain_id
            //add variables which are not in the var_def collection
        }

        public void addDataset()
        {
            //For a new dataset 
            //input: datasetSTO
            //Extract from datasetDTO the variables
            //new dataset
            //Iterate over datasetDTO variables
            //for each variable create new variable_Ref (use createOrRetrieve) add to datasets var_def collection
            //
        }


    }
}

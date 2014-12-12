using System.Collections.Generic;
using System.Linq;
using eTRIKS.Commons.Core.Application.Services;
using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.Core.Domain.Model.Templates;

namespace eTRIKS.Commons.Service.Services{
    public class TemplateService : ITemplateService
    {

        private IServiceUoW _dataServiceUnit;

        //TODO: should be replaced with only one repository to include only the Aggregate Root (i.e. DomainTemplate)
        private readonly IRepository<DomainDataset, string> _templateRepository;
        private readonly IRepository<DomainTemplateVariable, string> _templateVariableRepository;

        public TemplateService(IServiceUoW uoW){
            _dataServiceUnit = uoW;
            _templateRepository = uoW.GetRepository<DomainDataset, string>();
            _templateVariableRepository = uoW.GetRepository<DomainTemplateVariable,string>();
        }

        public IEnumerable<DomainDataset> GetAllDomains()
        {
            return _templateRepository.GetAll().ToList();
        }

        public DomainDataset GetDomainTemplateById(string oid)
        {
            return _templateRepository.GetById(oid);
        }

        // Customised Code
        public DomainDataset GetDomainTemplateDetailsById(string oid)
        {
            return null;
        }


        public DomainDataset GetDomainWithVariables(string oid)
        {
            //TODO:figure out the query for that in IRepository
           return _templateRepository.GetById(oid);
        }

        public string Gettestdomain(string something)
        {
            return something;
        }
    }
}

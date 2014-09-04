using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eTRIKS.Commons.App.DTOs;
using eTRIKS.Commons.Core.Domain.Model.Templates;
using eTRIKS.Commons.Core.Interfaces;
using eTRIKS.Commons.Core.Domain.Model;

namespace eTRIKS.Commons.App.Services{
    class TemplateService : IeTRIKSservice
    {

        private IServiceUoW dataServiceUnit;

        //TODO: should be replaced with only one repository to include only the Aggregate Root (i.e. DomainTemplate)
        private readonly IRepository<DomainTemplate,string> _templateRepository;
        private readonly IRepository<DomainTemplateVariable, string> _templateVariableRepository;

        TemplateService(IServiceUoW uoW){
            dataServiceUnit = uoW;
            _templateRepository = uoW.GetRepository<DomainTemplate, string>();
            _templateVariableRepository = uoW.GetRepository<DomainTemplateVariable,string>();
        }

        public IEnumerable<DomainTemplate> GetAllDomains()
        {
            return _templateRepository.GetAll().ToList();
        }

        public DomainTemplate GetDomainTemplateById(string oid)
        {
            return _templateRepository.GetById(oid);
        }

        public DomainTemplate GetDomainWithVariables(string oid)
        {
            //TODO:figure out the query for that in IRepository
           return _templateRepository.GetById(oid);
        }

    }
}

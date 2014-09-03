using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Model.Templates;
using eTRIKS.Commons.Core.Interfaces;
using eTRIKS.Commons.Core.Domain.Model;

namespace eTRIKS.Commons.App.Services{
    class TemplateService : IeTRIKSservice{

        private readonly IRepository<DomainTemplate,string> _templateRepository;

        TemplateService(IRepository<DomainTemplate,string> templateRepository){
            _templateRepository = templateRepository;
        }

        public IEnumerable<DomainTemplate> GetAllDomains()
        {
            return _templateRepository.GetAll().ToList();
        }

        public DomainTemplate GetDomainWithVariables(string oid)
        {
            //TODO:figure out the query for that in IRepository
           return _templateRepository.GetById(oid);
        }

    }
}

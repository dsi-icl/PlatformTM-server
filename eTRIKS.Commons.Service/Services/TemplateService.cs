using System.Collections.Generic;
using System.Linq;
using eTRIKS.Commons.Core.Application.Services;
using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.Core.Domain.Model.Templates;
using eTRIKS.Commons.Core.Domain.Model.ControlledTerminology;

namespace eTRIKS.Commons.Service.Services{
    public class TemplateService : ITemplateService
    {

        private IServiceUoW _dataServiceUnit;

        //TODO: should be replaced with only one repository to include only the Aggregate Root (i.e. DomainTemplate)
        private readonly IRepository<DomainTemplate,string> _templateRepository;
        private readonly IRepository<DomainVariableTemplate, string> _templateVariableRepository;
        private readonly IRepository<CVterm, string> _cvTermRepository;
        private readonly IRepository<Dictionary, string> _dictionaryRepository;

        public TemplateService(IServiceUoW uoW){
            _dataServiceUnit = uoW;
            _templateRepository = uoW.GetRepository<DomainTemplate, string>();
            _templateVariableRepository = uoW.GetRepository<DomainVariableTemplate,string>();
            _cvTermRepository = uoW.GetRepository<CVterm, string>();
            _dictionaryRepository = uoW.GetRepository<Dictionary, string>();
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

        public string getOIDOfCVterm(string name)
        {
            if (name.Length < 1)
                return null;
            return _cvTermRepository.GetRecords(o => o.Name.Equals(name)).First().OID;
        }

        public string Gettestdomain(string something)
        {
            return something;
        }

        public string checkDictionaryItem(string OID)
        {
            return _dictionaryRepository.GetRecords(o => o.OID.Equals(OID)).First().OID;
        }

        public void addDictionaryItem(Dictionary dictionaryItem)
        {
            _dictionaryRepository.Insert(dictionaryItem);
            _dataServiceUnit.Save();
        }

        public void addCVTerm(CVterm cvTerm)
        {
            _cvTermRepository.Insert(cvTerm);
            _dataServiceUnit.Save();
        }

        public string addDomainTemplate(List<DomainTemplate> dtList)
        {
            for (int i = 0; i < dtList.Count; i++)
            {
                _templateRepository.Insert(dtList[i]);
            }
            return _dataServiceUnit.Save();
        }

        public string updateDomainTemplate(List<DomainTemplate> dtList)
        {
            for (int i = 0; i < dtList.Count; i++)
            {
                _templateRepository.Update(dtList[i]);
            }
            return _dataServiceUnit.Save();
        }

        public string addDomainTemplateVariables(List<DomainVariableTemplate> dvtList)
        {
            for (int i = 0; i < dvtList.Count; i++)
            {
                _templateVariableRepository.Insert(dvtList[i]);
            }
            return _dataServiceUnit.Save();
        }

        public string updateDomainTemplateVariables(List<DomainVariableTemplate> dvtList)
        {
            for (int i = 0; i < dvtList.Count; i++)
            {
                _templateVariableRepository.Update(dvtList[i]);
            }
            return _dataServiceUnit.Save();
        }
    }
}

using System.Collections.Generic;
using eTRIKS.Commons.Core.Domain.Model.Templates;

namespace eTRIKS.Commons.Core.Application.Services
{
    public interface ITemplateService
    {
        IEnumerable<DomainTemplate> GetAllDomains();
        DomainTemplate GetDomainTemplateById(string oid);
        DomainTemplate GetDomainWithVariables(string oid);
    }
}
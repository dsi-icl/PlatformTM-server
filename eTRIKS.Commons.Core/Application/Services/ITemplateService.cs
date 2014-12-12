using System.Collections.Generic;
using eTRIKS.Commons.Core.Domain.Model.Templates;

namespace eTRIKS.Commons.Core.Application.Services
{
    public interface ITemplateService
    {
        IEnumerable<DomainDataset> GetAllDomains();
        DomainDataset GetDomainTemplateById(string oid);
        DomainDataset GetDomainWithVariables(string oid);
    }
}
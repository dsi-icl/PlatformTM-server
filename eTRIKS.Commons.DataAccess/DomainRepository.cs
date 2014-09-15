using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.Core.Domain.Model.Templates;

namespace eTRIKS.Commons.DataAccess
{
    class DomainRepository : GenericRepository<DomainTemplate, string> , IRepository<DomainTemplate, string>
    {
        public DomainRepository(DbContext dataContext) : base(dataContext)
        {
            //TODO: Implement here methods for getDomain, getDomainWithVariables ??
        }
    }
}

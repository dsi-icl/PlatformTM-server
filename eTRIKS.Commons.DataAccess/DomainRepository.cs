using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.Core.Domain.Model.Templates;

namespace eTRIKS.Commons.DataAccess
{
    class DomainRepository : GenericRepository<DomainTemplate, string>
    {
        public DomainRepository(DbContext dataContext) : base(dataContext)
        {
            //TODO: Implement here methods for getDomain, getDomainWithVariables ??

            
        }



        public void getDomainWithVariables()
        {
            
        }

        public void addVariableToDomain()
        {
            
        }
    }
}

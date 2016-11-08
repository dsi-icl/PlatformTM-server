using Microsoft.EntityFrameworkCore;
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

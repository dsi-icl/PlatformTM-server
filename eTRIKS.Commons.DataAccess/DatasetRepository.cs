using Microsoft.EntityFrameworkCore;
using eTRIKS.Commons.Core.Domain.Model;

namespace eTRIKS.Commons.DataAccess
{
    class DatasetRepository : GenericRepository<Dataset, int>
    {
       
        
        public DatasetRepository(DbContext dataContext) : base(dataContext)
        {
            DataContext = dataContext;
        }

        public DatasetRepository(DbSet<Dataset> entities) : base(entities)
        {
            
        }

        /// <summary>
        /// Returns dataset with variables resulting from merge between Variable_def and DomainVariable
        /// Variables from Varaible_Def will have Variable_REf attributes, variables from domainVariables will not
        /// 
        /// Dataset{
        /// "Domain":{name:..., class:...},
        /// "Variables":[
        ///     {"name":..., ...etc , ordernumber:...., required:..., sequence:...}
        /// ]
        /// } 
        /// </summary>
        //public void GetMergedDataset()
        //{
        //    DbSet doaminVariableSet = DataContext.Set<DomainTemplateVariable>();
        //    Entities
        //        .Include(d => d.Domain)
        //        .Include(d => d.Variables
        //            .Select(v => v.Variable))
        //        .Join(
        //          //  doaminVariableSet)
        //         ;
        //}
    }
}

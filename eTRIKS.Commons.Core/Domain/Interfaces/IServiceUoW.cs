using System;
using eTRIKS.Commons.Core.Domain.Model.Base;
using eTRIKS.Commons.Core.Domain.Model.Data.SDTM;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Application.AccountManagement;

namespace eTRIKS.Commons.Core.Domain.Interfaces
{
    public interface IServiceUoW : IDisposable
    {
        
        IRepository<TEntity, TPrimaryKey> GetRepository<TEntity, TPrimaryKey>()
            where TEntity : Identifiable<TPrimaryKey>, IEntity<TPrimaryKey>;

        IUserRepository GetUserRepository();

        IUserAccountRepository GetUserAccountRepository(); 

        void AddClassMap(string fieldname, string propertyName);

        void setSDTMentityDescriptor(SdtmRowDescriptor descriptor);

        string Save();

        Task<int> SaveChangesAsync();

    }
}

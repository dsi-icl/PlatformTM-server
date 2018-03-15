using System;
using System.Threading.Tasks;
using PlatformTM.Core.Application.AccountManagement;
using PlatformTM.Core.Domain.Model.Base;

namespace PlatformTM.Core.Domain.Interfaces
{
    public interface IServiceUoW : IDisposable
    {
        
        IRepository<TEntity, TPrimaryKey> GetRepository<TEntity, TPrimaryKey>()
            where TEntity : Identifiable<TPrimaryKey>, IEntity<TPrimaryKey>;

        IUserRepository GetUserRepository();

        IUserAccountRepository GetUserAccountRepository(); 

        string Save();

        Task<int> SaveChangesAsync();

        ICacheRepository<TEntity> GetCacheRepository<TEntity>() where TEntity : class;
    }
}

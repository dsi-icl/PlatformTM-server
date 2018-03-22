using System;
using eTRIKS.Commons.Core.Domain.Model.Base;
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

        string Save();

        Task<int> SaveChangesAsync();

        ICacheRepository<TEntity> GetCacheRepository<TEntity>() where TEntity : class;
    }
}

using System;
using System.Threading.Tasks;
namespace eTRIKS.Commons.Core.Domain.Interfaces
{
    public interface IUserRepository<TEntity,TResult>
    {
        Task<TEntity> FindUser(string userName, string password);
        Task<TResult> RegisterUser(TEntity entity, string password);
    }
}

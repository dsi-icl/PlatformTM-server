using System;
using System.Threading.Tasks;
namespace eTRIKS.Commons.Core.Domain.Interfaces
{
    public interface IUserRepository<TEntity>
    {
        Task<TEntity> FindUser(string userName, string password);
        Task<bool> RegisterUser(TEntity entity, string password);
    }
}

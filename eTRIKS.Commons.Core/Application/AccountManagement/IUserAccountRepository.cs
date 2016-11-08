using eTRIKS.Commons.Core.Domain.Interfaces;
using System;
using System.Threading.Tasks;

namespace eTRIKS.Commons.Core.Application.AccountManagement
{
    public interface IUserAccountRepository:  IRepository<Account, Guid> 
    {
        Account FindByUserName(string username);
        Task<Account> FindByUserNameAsync(string username);
        //Task<TEntity> FindByUserNameAsync(CancellationToken cancellationToken, string username);

        Account FindByEmail(string email);
        Task<Account> FindByEmailAsync(string email);
        //Task<User> FindByEmailAsync(CancellationToken cancellationToken, string email);
    }
}

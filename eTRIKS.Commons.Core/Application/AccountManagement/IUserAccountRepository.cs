using eTRIKS.Commons.Core.Domain.Interfaces;
using System;
using System.Threading.Tasks;

namespace eTRIKS.Commons.Core.Application.AccountManagement
{
    public interface IUserAccountRepository:  IRepository<UserAccount, Guid> 
    {
        UserAccount FindByUserName(string username);
        Task<UserAccount> FindByUserNameAsync(string username);
        //Task<TEntity> FindByUserNameAsync(CancellationToken cancellationToken, string username);

        UserAccount FindByEmail(string email);
        Task<UserAccount> FindByEmailAsync(string email);
        //Task<User> FindByEmailAsync(CancellationToken cancellationToken, string email);
    }
}

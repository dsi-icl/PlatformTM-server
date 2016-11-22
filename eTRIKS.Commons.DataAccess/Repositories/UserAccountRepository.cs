using eTRIKS.Commons.Core.Application.AccountManagement;
using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace eTRIKS.Commons.DataAccess
{
    public class UserAccountRepository : GenericRepository<UserAccount, Guid>, IUserAccountRepository
    {

        public UserAccountRepository(DbContext _ctx) : base(_ctx)
        {
            DataContext = _ctx;
            //DataContext.Configuration.ProxyCreationEnabled = false;
            //Entities = DataContext.Set<TEntity>();
        }

        public UserAccount FindByEmail(string email)
        {
            return Entities.FirstOrDefault(a => a.User.Email == email);
        }

        public Task<UserAccount> FindByEmailAsync(string email)
        {
            return Entities.FirstOrDefaultAsync(a => a.User.Email == email);
        }

        public UserAccount FindByUserName(string username)
        {
            return Entities.FirstOrDefault(a => a.UserName == username);
        }

        public Task<UserAccount> FindByUserNameAsync(string username)
        {
            return Entities.FirstOrDefaultAsync(a => a.UserName == username);
        }
    }
}

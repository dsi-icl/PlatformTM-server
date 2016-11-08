using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.Core.Domain.Model.Users;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace eTRIKS.Commons.DataAccess
{
    public class UserRepository : GenericRepository<User,Guid>, IUserRepository
    {
        //private UserManager<IdentityUser> _userManager;
        //private ApplicationUserManager userManager;

        public UserRepository(DbContext _ctx) : base(_ctx)
        {
            //_ctx = new AuthContext();
            //_userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(_ctx));
            //userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(_ctx));
        }

        public User FindByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public Task<User> FindByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public User FindByUserName(string username)
        {
            throw new NotImplementedException();
        }

        public Task<User> FindByUserNameAsync(string username)
        {
            throw new NotImplementedException();
        }

        //public UserAuthRepository()
        //{
        //    //IdentityDbContext _ctx = new AuthContext();
        //   // _userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(_ctx));
        //}
        //public async Task<IdentityResult> RegisterUser(ApplicationUser user, string password)
        //{
        //    IdentityResult addUserResult=null;
        //    try
        //    {
        //        addUserResult = await userManager.CreateAsync(user, password);

        //    }
        //    catch (Microsoft.EntityFrameworkCore.Validation.DbEntityValidationException ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine(ex);
        //    }


        //    return addUserResult;
        //}

        //public async Task<ApplicationUser> FindUser(string userName, string password)
        //{
        //    ApplicationUser user = await userManager.FindAsync(userName, password);

        //    return user;
        //}

        //Task<User> IUserRepository<User, Guid>.FindUser(string userName, string password)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<TResult> RegisterUser(User entity, string password)
        //{
        //    throw new NotImplementedException();
        //}

    }
}

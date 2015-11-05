using eTRIKS.Commons.Core.Application.Services.UserAuthentication;
using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.DataAccess.UserManagement;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTRIKS.Commons.DataAccess
{
    public class UserAuthRepository<TEntity> : IUserRepository<ApplicationUser>
    {
        //private UserManager<IdentityUser> _userManager;
        private ApplicationUserManager userManager;

        public UserAuthRepository(IdentityDbContext<ApplicationUser> _ctx)
        {
            //_ctx = new AuthContext();
            //_userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(_ctx));
            userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(_ctx));
        }

        public UserAuthRepository()
        {
            //IdentityDbContext _ctx = new AuthContext();
           // _userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(_ctx));
        }
        public async Task<bool> RegisterUser(ApplicationUser user, string password)
        {
            try
            {
                IdentityResult addUserResult = await userManager.CreateAsync(user, password);
                return addUserResult.Succeeded;
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }


            return false;
        }

        public async Task<ApplicationUser> FindUser(string userName, string password)
        {
            ApplicationUser user = await userManager.FindAsync(userName, password);

            return user;
        }


       
    }
}

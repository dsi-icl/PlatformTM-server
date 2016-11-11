using eTRIKS.Commons.Core.Application.AccountManagement;
using eTRIKS.Commons.Core.Domain.Model.Users;
using eTRIKS.Commons.Service.DTOs;
using eTRIKS.Commons.Service.Services.Authentication;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace eTRIKS.Commons.Service.Services.UserManagement
{
    public class UserAccountService : UserManager<UserAccount, Guid>
    {
        //private readonly ApplicationUserManager _userManager;

        public UserAccountService(IUserStore<UserAccount, Guid> store) : base(store)
        {

        }

        

        public async Task<IdentityResult> RegisterUser(UserDTO userDTO)
        {
            var Account = new Account()
            {
                UserName = userDTO.Username,
                JoinDate = DateTime.Now.Date,
                TwoFactorEnabled = true,
                PSK = TimeSensitivePassCodeService.GeneratePresharedKey(),
                User = new User()
                {
                    Email = userDTO.Email,
                    FirstName = userDTO.FirstName,
                    LastName = userDTO.LastName,
                    Organization = userDTO.Organization
                }
        };

            var userAccount = new UserAccount(Account);

            var result = await this.CreateAsync(userAccount, userDTO.Password);
            return result;
        }

        public async Task<UserAccount> FindUserAsync(string name, string password)
        {
            return await this.FindAsync(name, password);
        }


        public async Task<string> GetUserPsk(UserDTO userDTO)
        {
            string psk = null;
            var user = await this.FindByNameAsync(userDTO.Username);
            if (user != null)
            {
                psk = user.Account.PSK;
            }
            return psk;
        }
    }
}

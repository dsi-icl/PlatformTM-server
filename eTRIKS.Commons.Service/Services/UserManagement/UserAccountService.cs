using eTRIKS.Commons.Core.Application.AccountManagement;
using eTRIKS.Commons.Core.Domain.Model.Users;
using eTRIKS.Commons.Service.DTOs;
using eTRIKS.Commons.Service.Services.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eTRIKS.Commons.Service.Services.UserManagement
{
    public class UserAccountService
    {
        private readonly UserManager<UserAccount> _userManager;
        private readonly SignInManager<UserAccount> _signInManager;
       // private readonly RoleManager<UserRole> _roleManager;

        public UserAccountService(UserManager<UserAccount> userManager, SignInManager<UserAccount> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;

        }

        //    public UserAccountService(IUserStore<UserAccount> store, IOptions<IdentityOptions> optionsAccessor,
        //    IPasswordHasher<UserAccount> passwordHasher,
        //    IEnumerable<IUserValidator<UserAccount>> userValidators,
        //    IEnumerable<IPasswordValidator<UserAccount>> passwordValidators,
        //    ILookupNormalizer keyNormalizer,
        //    IdentityErrorDescriber errors,
        //    IEnumerable<IUserTokenProvider<UserAccount>> tokenProviders,
        //    ILoggerFactory logger,
        //    IHttpContextAccessor contextAccessor) :
        //    base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, tokenProviders, logger, contextAccessor)
        //    {

        //    }



        public async Task<IdentityResult> RegisterUser(UserDTO userDTO)
        {
            var userAccount = new UserAccount()
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
            var result = await _userManager.CreateAsync(userAccount, userDTO.Password);
            return result;
        }

        //public async Task<UserAccount> FindUserAsync(string name, string password)
        //{
        //    return await _userManager.FindAsync(name, password);
        //}

        public async Task<SignInResult> SignIn(UserDTO userDTO)
        {
            SignInResult result = await _signInManager.PasswordSignInAsync(userDTO.Username, userDTO.Password, false, false);
            return result;
        }

        public async Task<bool> CheckUser(UserDTO userDTO)
        {
            var user = await _userManager.FindByNameAsync(userDTO.Username);

            if (user != null)
                return await _userManager.CheckPasswordAsync(user, userDTO.Password);
            return false;
        }


        public async Task<string> GetUserPsk(UserDTO userDTO)
        {
            string psk = null;
            var user = await _userManager.FindByNameAsync(userDTO.Username);
            if (user != null)
            {
                psk = user.PSK;
            }
            return psk;
        }
    }
}

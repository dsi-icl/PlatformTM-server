using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using PlatformTM.Core.Application.AccountManagement;
using PlatformTM.Core.Domain.Model.Users;
using PlatformTM.Services.DTOs;
using PlatformTM.Services.Services.Authentication;

namespace PlatformTM.Services.Services.UserManagement
{
    public class UserAccountService
    {
        private readonly UserManager<UserAccount> _userManager;
        private readonly SignInManager<UserAccount> _signInManager;

        public UserAccountService(UserManager<UserAccount> userManager, SignInManager<UserAccount> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;

        }

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
            try
            {
                userAccount.Claims.Add(new UserClaim() { ClaimType = "FirstName", ClaimValue = userDTO.FirstName });
                userAccount.Claims.Add(new UserClaim() { ClaimType = ClaimTypes.UserData, ClaimValue = userAccount.User.Id.ToString()});
                return  await _userManager.CreateAsync(userAccount, userDTO.Password);
            }
            catch (Exception e)
            {
               return IdentityResult.Failed(new IdentityError() {Code = "",Description = e.Message});
            }
        }

        public async Task<UserAccount> FindUserAsync(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null) return null;
            return await _userManager.CheckPasswordAsync(user, password) ? user : null;
        }

        public async Task<UserAccount> FindByIdAsync(string userId){
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<UserDTO> GetUserInfo(string userId)
        {
            var useraccount = await _userManager.FindByIdAsync(userId);
            if(useraccount == null)
                return null;
            var userDTO = new UserDTO()
            {
               FirstName = useraccount.User.FirstName,
               Email = useraccount.User.Email,
               Username = useraccount.UserName
            };
            return userDTO;
        }

        public async Task<ClaimsPrincipal> GetClaimsPrincipleForUser(UserAccount userAccount)
        {
           var cp =  await _signInManager.CreateUserPrincipalAsync(userAccount);
            //cp.Claims.Append()
            //var claims = cp.Claims.ToList();
            //claims.Add(new Claim(ClaimTypes.UserData, userAccount.UserId.ToString()));
            //cp.Claims = claims;
            //OR CALL GetClaimsForUser
            return cp;
        }

        public async Task<List<string>> GetClaimsForUser(string userId)
        {
            var userAccount = await _userManager.FindByIdAsync(userId);
            if (userAccount == null)
                return null;
            var claims = await _userManager.GetClaimsAsync(userAccount);

            var permissions = claims.ToList().FindAll(c => c.Type == ClaimTypes.Role)?.Select(r=>r.Value);
            return permissions?.ToList();
        }

        public async Task<IdentityResult> AddUserRole(string rights, int projectId, string userId)
        {
            if(rights == "all")
            {
                var userAccount = await _userManager.FindByIdAsync(userId);
                if (userAccount == null)
                    return IdentityResult.Failed();

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Role, "can-manage-" + projectId), 
                    new Claim(ClaimTypes.Role, "can-manage-etl-" + projectId),
                    new Claim(ClaimTypes.Role, "can-manage-drive-" + projectId)

                };
                return await _userManager.AddClaimsAsync(userAccount,claims);

            }
            return IdentityResult.Failed();
        }

        public async Task SignInAsync(UserAccount user, bool isPersistent)
        {
            await _signInManager.SignInAsync(user, isPersistent: false);
        }

        public async Task<SignInResult> Login(string email, string password, bool rememberme=false, bool lockout=false){
            return await _signInManager.PasswordSignInAsync(email, password, rememberme, lockout);

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

        public async Task<string> GenerateEmailConfirmationTokenAsync(UserAccount user){
            
            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task<IdentityResult> ConfirmEmailAsync(UserAccount user, string token){
           return await _userManager.ConfirmEmailAsync(user, token);
        }
    }
}

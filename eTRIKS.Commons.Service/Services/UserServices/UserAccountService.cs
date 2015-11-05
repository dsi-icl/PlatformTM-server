using eTRIKS.Commons.Core.Application.Services.UserAuthentication;
using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.DataAccess.UserManagement;
using eTRIKS.Commons.Service.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTRIKS.Commons.Service.Services.UserServices
{
    public class UserAccountService
    {
        private IUserRepository<ApplicationUser> _userRepository;
        private IServiceUoW AppDBcontext;
        //private ApplicationUserManager userManager;

        public UserAccountService(IServiceUoW uoW)
        {
            AppDBcontext = uoW;
            _userRepository = uoW.GetUserRepository<ApplicationUser>();
        }

        public async Task<bool> RegisterUser(UserDTO userDTO)
        {
            ApplicationUser user = new ApplicationUser()
            {
                UserName = userDTO.Username,
                Email = userDTO.Email,
                FirstName = userDTO.FirstName,
                LastName = userDTO.LastName,
                Level = 3,
                JoinDate = DateTime.Now.Date,
                TwoFactorEnabled = true,
                PSK = TimeSensitivePassCodeService.GeneratePresharedKey()
            };
            

            bool result = await _userRepository.RegisterUser(user,userDTO.Password);
            return result;

            //IdentityResult addUserResult = userManager.CreateAsync(user, userDTO.Password);
        }

        public async Task<string> GetUserPSK(UserDTO userDTO)
        {
            string PSK = null;
            ApplicationUser user = await _userRepository.FindUser(userDTO.Username, userDTO.Password);
            if (user != null)
            {
                PSK = user.PSK;
            }
            return PSK;
        }
    }
}

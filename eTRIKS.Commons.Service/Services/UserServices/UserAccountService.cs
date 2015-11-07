using eTRIKS.Commons.Core.Application.Services.UserAuthentication;
using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.DataAccess.UserManagement;
using eTRIKS.Commons.Service.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace eTRIKS.Commons.Service.Services.UserServices
{
    public class UserAccountService
    {
        private IUserRepository<ApplicationUser,IdentityResult> _userRepository;
        private IServiceUoW AppDBcontext;
        //private ApplicationUserManager userManager;

        public UserAccountService(IServiceUoW uoW)
        {
            AppDBcontext = uoW;
            _userRepository = uoW.GetUserRepository<ApplicationUser,IdentityResult>();
        }

        public async Task<IdentityResult> RegisterUser(UserDTO userDTO)
        {
            var user = new ApplicationUser()
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


            var result = await _userRepository.RegisterUser(user, userDTO.Password);
            return result;
        }

        public async Task<string> GetUserPsk(UserDTO userDTO)
        {
            string psk = null;
            var user = await _userRepository.FindUser(userDTO.Username, userDTO.Password);
            if (user != null)
            {
                psk = user.PSK;
            }
            return psk;
        }
    }
}

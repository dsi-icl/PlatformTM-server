using eTRIKS.Commons.Core.Application.AccountManagement;
using eTRIKS.Commons.Core.Domain.Interfaces;
using System;
using System.Threading.Tasks;

namespace eTRIKS.Commons.Service.Services.UserManagement
{
    public class UserStore : IUserStore<UserAccount, Guid>, IUserPasswordStore<UserAccount,Guid>
    {
        private readonly IServiceUoW _unitOfWork;
        private readonly IUserRepository _userRepository;
        private readonly IUserAccountRepository _accountRepository;
        public UserStore(IServiceUoW uoW)
        {
            _unitOfWork = uoW;
            _userRepository = uoW.GetUserRepository();
            _accountRepository = uoW.GetUserAccountRepository();

        }
        public Task CreateAsync(UserAccount userAccount)
        {
            _userRepository.Insert(userAccount.Account.User);
            _accountRepository.Insert(userAccount.Account);
            return _unitOfWork.SaveChangesAsync();
        }

        public Task DeleteAsync(UserAccount user)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            //DI takes care of that
        }

        public async Task<UserAccount> FindByIdAsync(Guid userId)
        {
            Account appUser = await _accountRepository.GetAsync(userId);
            return appUser != null ? new UserAccount(appUser) : null;
        }

        public async Task<UserAccount> FindByNameAsync(string userName)
        {
            Account appuser = await _accountRepository.FindByUserNameAsync(userName);
            return appuser!=null ? new UserAccount(appuser) : null;
        }

        public Task UpdateAsync(UserAccount user)
        {
            throw new NotImplementedException();
        }

        public Task SetPasswordHashAsync(UserAccount user, string passwordHash)
        {
            user.Account.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        public Task<string> GetPasswordHashAsync(UserAccount user)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            return Task.FromResult<string>(user.Account.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(UserAccount user)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            return Task.FromResult<bool>(!string.IsNullOrWhiteSpace(user.Account.PasswordHash));
        }



        //private Account GetAccount(UserAccount userAccount)
        //{
        //    return new Account()
        //    {
        //        AdminApproved = userAccount.AdminApproved,
        //        JoinDate = userAccount.JoinDate,
        //        PasswordHash = userAccount.PasswordHash,
        //        UserName = userAccount.UserName,
        //        PSK = userAccount.PSK,
        //        SecurityStamp = userAccount.SecurityStamp,
        //        EmailConfirmed = userAccount.EmailConfirmed,
        //        TwoFactorEnabled = userAccount.TwoFactorEnabled,
        //        UserId = userAccount.UserId,
        //        Claims = userAccount.Claims,
        //        User = userAccount.User
        //    };
        //}
    }
}

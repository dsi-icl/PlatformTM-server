using eTRIKS.Commons.Core.Application.AccountManagement;
using eTRIKS.Commons.Core.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace eTRIKS.Commons.Service.Services.UserManagement
{
    public class UserStore : IUserStore<UserAccount>, IUserPasswordStore<UserAccount>
    {
        private readonly IServiceUoW _unitOfWork;
        private readonly IUserRepository _userRepository;
        private readonly IUserAccountRepository _accountRepository;
        private bool _disposed;

        public UserStore(IServiceUoW uoW)
        {
            _unitOfWork = uoW;
            _userRepository = uoW.GetUserRepository();
            _accountRepository = uoW.GetUserAccountRepository();

        }

        public Task SetPasswordHashAsync(UserAccount user, string passwordHash, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetPasswordHashAsync(UserAccount user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> HasPasswordAsync(UserAccount user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetUserIdAsync(UserAccount user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetUserNameAsync(UserAccount user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetUserNameAsync(UserAccount user, string userName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetNormalizedUserNameAsync(UserAccount user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedUserNameAsync(UserAccount user, string normalizedName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> CreateAsync(UserAccount userAccount, CancellationToken cancellationToken)
        {
            _userRepository.Insert(userAccount.User);
            _accountRepository.Insert(userAccount);
            _unitOfWork.SaveChangesAsync();
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> UpdateAsync(UserAccount user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> DeleteAsync(UserAccount user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<UserAccount> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<UserAccount> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
                //handle.Dispose();
            _disposed = true;
        }

        //public void Dispose()
        //{
        //    throw new NotImplementedException();
        //}
        //public Task CreateAsync(UserAccount userAccount)
        //{
        //    _userRepository.Insert(userAccount.Account.User);
        //    _accountRepository.Insert(userAccount.Account);
        //    return _unitOfWork.SaveChangesAsync();
        //}

        //public Task DeleteAsync(UserAccount user)
        //{
        //    throw new NotImplementedException();
        //}

        //public void Dispose()
        //{
        //    //DI takes care of that
        //}

        //public async Task<UserAccount> FindByIdAsync(Guid userId)
        //{
        //    Account appUser = await _accountRepository.GetAsync(userId);
        //    return appUser != null ? new UserAccount(appUser) : null;
        //}

        //public async Task<UserAccount> FindByNameAsync(string userName)
        //{
        //    Account appuser = await _accountRepository.FindByUserNameAsync(userName);
        //    return appuser!=null ? new UserAccount(appuser) : null;
        //}

        //public Task UpdateAsync(UserAccount user)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task SetPasswordHashAsync(UserAccount user, string passwordHash)
        //{
        //    user.Account.PasswordHash = passwordHash;
        //    return Task.FromResult(0);
        //}

        //public Task<string> GetPasswordHashAsync(UserAccount user)
        //{
        //    if (user == null)
        //        throw new ArgumentNullException("user");
        //    return Task.FromResult<string>(user.Account.PasswordHash);
        //}

        //public Task<bool> HasPasswordAsync(UserAccount user)
        //{
        //    if (user == null)
        //        throw new ArgumentNullException("user");
        //    return Task.FromResult<bool>(!string.IsNullOrWhiteSpace(user.Account.PasswordHash));
        //}



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

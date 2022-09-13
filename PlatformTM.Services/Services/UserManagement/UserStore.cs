using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PlatformTM.Core.Application.AccountManagement;
using PlatformTM.Core.Domain.Interfaces;

namespace PlatformTM.Models.Services.UserManagement
{
    public class UserStore : IUserPasswordStore<UserAccount>, IUserClaimStore<UserAccount>, IUserSecurityStampStore<UserAccount>, IUserEmailStore<UserAccount>
    {
        private readonly IServiceUoW _unitOfWork;
        private readonly IUserRepository _userRepository;
        private readonly IUserAccountRepository _accountRepository;
        public IdentityErrorDescriber ErrorDescriber { get; set; }
        private bool _disposed;

        public UserStore(IServiceUoW uoW, IdentityErrorDescriber describer = null)
        {
            _unitOfWork = uoW;
            _userRepository = uoW.GetUserRepository();
            _accountRepository = uoW.GetUserAccountRepository();
            ErrorDescriber = describer ?? new IdentityErrorDescriber();
        }

       

        public Task<string> GetUserIdAsync(UserAccount account, CancellationToken cancellationToken)
        {
            if (account == null)
                throw new ArgumentNullException(nameof(account));
            return Task.FromResult<string>(account.Id.ToString());
        }
        public Task<string> GetUserNameAsync(UserAccount account, CancellationToken cancellationToken)
        {
            if (account == null)
                throw new ArgumentNullException(nameof(account));
            return Task.FromResult<string>(account.UserName);
        }
        public Task SetUserNameAsync(UserAccount user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return Task.FromResult(true);
        }
        public Task<string> GetNormalizedUserNameAsync(UserAccount user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }
        public Task SetNormalizedUserNameAsync(UserAccount user, string normalizedName, CancellationToken cancellationToken)
        {
            user.UserName = normalizedName;
            return Task.FromResult(true);
        }
        public async Task<IdentityResult> CreateAsync(UserAccount userAccount, CancellationToken cancellationToken)
        {
            _userRepository.Insert(userAccount.User);
            _accountRepository.Insert(userAccount);
            var res = await _unitOfWork.SaveChangesAsync();
            return res > 0 ? IdentityResult.Success : IdentityResult.Failed();
        }
        public async Task<IdentityResult> UpdateAsync(UserAccount user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            try
            {
                 _accountRepository.Update(user);
                 await _unitOfWork.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
            }
            return IdentityResult.Success;
        }
        public Task<IdentityResult> DeleteAsync(UserAccount user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        public Task<UserAccount> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            var guidId = Guid.Parse(userId);
            return Task.FromResult(_accountRepository.FindSingle(u => u.Id == guidId, new List<string>() {"Claims","User"}));
        }
        public async Task<UserAccount> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {

                        return await _accountRepository.FindByUserNameAsync(normalizedUserName);
        }

    
        //Password Methods
        public Task SetPasswordHashAsync(UserAccount account, string passwordHash, CancellationToken cancellationToken)
        {
            account.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }
        public Task<string> GetPasswordHashAsync(UserAccount account, CancellationToken cancellationToken)
        {
            if (account == null)
                throw new ArgumentNullException("UserAccount");
            return Task.FromResult<string>(account.PasswordHash);
        }
        public Task<bool> HasPasswordAsync(UserAccount user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash != null);
        }


        //Claims Methods
        public Task<IList<Claim>> GetClaimsAsync(UserAccount userAccount, CancellationToken cancellationToken)
        {
            var account = _accountRepository.FindSingle(a=>a.UserName == userAccount.UserName, new List<string>() {"Claims"});
            var userClaims = new List<Claim>();
            if (account != null)
            {
                var claims = account.Claims;
                userClaims.AddRange(claims.Select(userClaim => new Claim(userClaim.ClaimType, userClaim.ClaimValue)));
            }
            return Task.FromResult((IList<Claim>) userClaims);
        }
        public Task ReplaceClaimAsync(UserAccount userAccount, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
           return Task.CompletedTask;
        }
        public Task<IList<UserAccount>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            return null;
        }
        public Task RemoveClaimsAsync(UserAccount userAccount, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }
        public async Task AddClaimsAsync(UserAccount userAccount, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            foreach (var claim in claims)
            {
                var userClaim = new UserClaim()
                {
                    ClaimType = claim.Type,
                    ClaimValue = claim.Value
                };
                userAccount.Claims.Add(userClaim);
            }

            _accountRepository.Update(userAccount);
            await _unitOfWork.SaveChangesAsync();

        }


        //Security Stamp Methods
        public Task SetSecurityStampAsync(UserAccount user, string stamp, CancellationToken cancellationToken)
        {
            user.SecurityStamp = stamp;
            return Task.FromResult(0);
        }
        public Task<string> GetSecurityStampAsync(UserAccount user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.SecurityStamp);
        }




        public Task SetEmailAsync(UserAccount userAccount, string email, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (userAccount == null)
            {
                throw new ArgumentNullException(nameof(userAccount));
            }
            userAccount.User.Email = email;
            return Task.CompletedTask;
        }

        public Task<string> GetEmailAsync(UserAccount userAccount, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (userAccount == null)
            {
                throw new ArgumentNullException(nameof(userAccount));
            }
            return Task.FromResult(userAccount.User.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(UserAccount user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(UserAccount user, bool confirmed, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
        
            user.EmailConfirmed = confirmed;
            return Task.CompletedTask;
        }

        public async Task<UserAccount> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            var account = await _accountRepository.FindAsync(u => u.User.Email == normalizedEmail);
            return account;

        }

        public Task<string> GetNormalizedEmailAsync(UserAccount userAccount, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (userAccount == null)
            {
                throw new ArgumentNullException(nameof(userAccount));
            }
            return Task.FromResult(userAccount.User.Email);
        }

        public Task SetNormalizedEmailAsync(UserAccount userAccount, string normalizedEmail, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (userAccount == null)
            {
                throw new ArgumentNullException(nameof(userAccount));
            }
            //userAccount.User.NormalizedEmail = normalizedEmail;
            return Task.CompletedTask;
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

        protected void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

    }
}

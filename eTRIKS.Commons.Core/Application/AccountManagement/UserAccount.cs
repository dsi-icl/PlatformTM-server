using eTRIKS.Commons.Core.Domain.Model.Base;
using eTRIKS.Commons.Core.Domain.Model.Users;
using System;
using System.Collections.Generic;

namespace eTRIKS.Commons.Core.Application.AccountManagement
{
    public class UserAccount : Identifiable<Guid>
    {
        public string UserName { get; set; }
        public virtual bool EmailConfirmed { get; set; }
        public virtual string PasswordHash { get; set; }
        public virtual string SecurityStamp { get; set; }
        public virtual bool TwoFactorEnabled { get; set; }
        public virtual bool AdminApproved { get; set; }
        public DateTime JoinDate { get; set; }
        public string PSK { get; set; }

        public User User { get; set; }
        public Guid UserId { get; set; }
        private ICollection<Claim> _claims;

        public virtual ICollection<Claim> Claims
        {
            get { return _claims ?? (_claims = new List<Claim>()); }
            set { _claims = value; }
        }
        public UserAccount()
        {
            Id = Guid.NewGuid();
        }
    }
}

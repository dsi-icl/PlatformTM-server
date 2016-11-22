using eTRIKS.Commons.Core.Domain.Model.Base;
using System;

namespace eTRIKS.Commons.Core.Application.AccountManagement
{
    public class Claim : Identifiable<int>
    {
        

        public virtual Guid UserId { get; set; }
        public virtual string ClaimType { get; set; }
        public virtual string ClaimValue { get; set; }

        private UserAccount _user;
        public virtual UserAccount User
        {
            get { return _user; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                _user = value;
                UserId = value.Id;
            }
        }
    }
}

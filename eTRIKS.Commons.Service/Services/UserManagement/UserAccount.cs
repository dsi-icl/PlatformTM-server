using eTRIKS.Commons.Core.Application.AccountManagement;
using System;
namespace eTRIKS.Commons.Service.Services.UserManagement
{
    public class UserAccount : IUser<Guid>
    {
        public Account Account { get; set; }
        public  Guid Id
        {
            get
            {
                return Account.Id;
            }
        }

        public  string UserName
        {
            get
            {
                return Account.UserName;
            }

            set
            {
                Account.UserName = value;
            }
        }

        public UserAccount(Account account)
        {
            if(account!=null)
                Account = account;
        }
    }
}

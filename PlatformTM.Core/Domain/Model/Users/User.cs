using System;
using System.Collections.Generic;
using PlatformTM.Core.Domain.Model.Base;
using PlatformTM.Core.JoinEntities;

namespace PlatformTM.Core.Domain.Model.Users
{
    public class User : Identifiable<Guid> 
    {

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Organization { get; set; }

        public string Email { get; set; }

        //public List<UserDataset> Datasets { get; set; }

        public IList<ProjectUser> AffiliatedProjects { get; set; }

      public User()
        {
            this.Id = Guid.NewGuid();
           }
    }
}

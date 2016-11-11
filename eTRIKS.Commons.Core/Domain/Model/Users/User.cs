using eTRIKS.Commons.Core.Domain.Model.Base;
using eTRIKS.Commons.Core.Domain.Model.Users.Datasets;
using System;
using System.Collections.Generic;

namespace eTRIKS.Commons.Core.Domain.Model.Users
{
    public class User : Identifiable<Guid> 
    {

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Organization { get; set; }

        public string Email { get; set; }

        //public List<UserDataset> Datasets { get; set; }

        public List<Project> AffiliatedProjects { get; set; }
        public User()
        {
            this.Id = Guid.NewGuid();
        }
    }
}

using System;
using eTRIKS.Commons.Core.Domain.Model.Base;
using System.Collections.Generic;
using eTRIKS.Commons.Core.Domain.Model.Users;

namespace eTRIKS.Commons.Core.Domain.Model
{
    public class Project : Identifiable<int>
    {
        public string Name { get; set;}
        public string Description { get; set;}
        public string Accession { get; set;}
        public ICollection<Study> Studies { get; set;}
        public ICollection<DataFile> DataFiles { get; set; } 

        public Project()
        {
            Studies = new List<Study>();
        }

        public ICollection<Activity> Activities { get; set; }
        public IList<User> Users { get; set; }
        public User Owner { get; set; }
        public Guid OwnerId { get; set; }
    }
}

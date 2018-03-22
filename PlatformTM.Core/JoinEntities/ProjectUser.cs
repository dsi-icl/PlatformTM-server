using System;
using PlatformTM.Core.Domain.Model;
using PlatformTM.Core.Domain.Model.Users;

namespace PlatformTM.Core.JoinEntities
{
    public class ProjectUser
    {
        public Guid UserId { get; set; }
        public User User { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; }
    }
}

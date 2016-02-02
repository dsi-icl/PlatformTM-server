

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eTRIKS.Commons.Core.Domain.Model.Base;

namespace eTRIKS.Commons.Core.Domain.Model
{
    public class Activity : Identifiable<int>
    {
        //public string OID { get; set; }
        public string Name { get; set; }
        public int ProjectId { get; set; }

        public ICollection<Dataset> Datasets { get; set; }
        //public List<Study> Studies { get; set; }
        public Project Project { get; set; }
       

    }
}
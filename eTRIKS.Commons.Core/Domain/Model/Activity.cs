using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eTRIKS.Commons.Core.Domain.Model
{
    public class Activity : Identifiable
    {
        //public string OID { get; set; }
        public string Name { get; set; }
        public string StudyId { get; set; }

        public ICollection<Dataset> Datasets { get; set; }
        public Study Study { get; set; }
    }
}
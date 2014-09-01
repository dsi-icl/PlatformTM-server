using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eTRIKS.Commons.Core.Domain.Model
{
    public class Dataset:Identifiable
    {
       // public string DatasetId { get; set; }
        public string DataFile { get; set; }
        public string ActivityId { get; set; }
        public string DomainId { get; set; }
        public Activity Activity { get; set; }
        public DomainTemplate Domain { get; set; }
        public ICollection<VariableReference> Variables { get; set; }

    }
}
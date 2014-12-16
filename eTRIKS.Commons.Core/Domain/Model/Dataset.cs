using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eTRIKS.Commons.Core.Domain.Model.Base;
using eTRIKS.Commons.Core.Domain.Model.Templates;

namespace eTRIKS.Commons.Core.Domain.Model
{
    public class Dataset : Identifiable<string>
    {
        //public string DatasetId { get; set; }
        public string DataFile { get; set; }
        public string ActivityId { get; set; }
        public string DomainId { get; set; }
        public Activity Activity { get; set; }
        public DomainTemplate Domain { get; set; }
        public ICollection<VariableReference> Variables { get; set; }

    }
}
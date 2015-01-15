using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eTRIKS.Commons.Core.Domain.Model.Base;
using eTRIKS.Commons.Core.Domain.Model.Templates;

namespace eTRIKS.Commons.Core.Domain.Model
{
    public class Dataset : Identifiable<int>
    {
        public string DataFile { get; set; }
        public int ActivityId { get; set; }
        public string DomainId { get; set; }
        public Activity Activity { get; private set; }
        public DomainTemplate Domain { get; set; }
        public ICollection<VariableReference> Variables { get; set; }

        public Dataset()
        {
            Variables = new List<VariableReference>();
        }
    }
}
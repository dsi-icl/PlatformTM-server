using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eTRIKS.Commons.Core.Domain.Model
{
    public class DomainTemplate : Identifiable
    {
        //public string OID { get; set; }
        public string Name { get; set; }
        public string Class { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public string Structure { get; set; }
        public Boolean IsRepeating { get; set; }

        public ICollection<DomainTemplateVariable> Variables { get; set; }
    }

}
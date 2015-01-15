using eTRIKS.Commons.Core.Domain.Model.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eTRIKS.Commons.Core.Domain.Model.ControlledTerminology
{
    public class Dictionary : Identifiable<string>
    {
        //public string OID { get; set; }
        public string Name { get; set; }
        public string Definition { get; set; }

        public string XrefId { get; set; }
        public Dbxref Xref { get; set; }

        public virtual ICollection<CVterm> Terms { get; set; }

    }
}
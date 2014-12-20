using eTRIKS.Commons.Core.Domain.Model.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eTRIKS.Commons.Core.Domain.Model.ControlledTerminology
{
    public class CVterm : Identifiable<string>
    {
       // public string OID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Definition { get; set; }
        //public Nullable<int> Order { get; set; }
        //public Nullable<int> Rank { get; set; }
        public Nullable<bool> IsUserSpecified { get; set; }
        public String Synonyms { get; set; }
        public string DictionaryId { get; set; }
        public string XrefId { get; set; }
        public virtual Dbxref Xref { get; set; }
        public virtual Dictionary Dictionary { get; private set; }

    }
}
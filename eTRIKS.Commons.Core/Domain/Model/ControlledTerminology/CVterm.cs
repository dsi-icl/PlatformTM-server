using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eTRIKS.Commons.Core.Domain.Model.ControlledTerminology
{
    public class CVterm
    {
        //Investigate what should be primary key for CVterm
        public string Code { get; set; }
        public string Name { get; set; }
        public Nullable<int> Order { get; set; }
        public Nullable<int> Rank { get; set; }
        public Nullable<bool> UserSpecified { get; set; }
        public string DictionartyId { get; set; }
        public string externalReferencId { get; set; }
        public virtual DBxref externalReference { get; set; }
        public virtual Dictionary Dictionary { get; set; }

    }
}
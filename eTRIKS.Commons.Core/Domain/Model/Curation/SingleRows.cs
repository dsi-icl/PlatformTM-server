using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Model.Base;

namespace eTRIKS.Commons.Core.Domain.Model.Curation
{
    public class SingleRow : Identifiable<Guid>
    {
        public Dictionary<string, string> RowValues{ get; set; }

        public int FileId { get; set; }
        //public Dictionary<string, string>  Type { get; set; } 
       

        //public List<string> rowValues;
        //public HeaderElements headers;
    }
}
 
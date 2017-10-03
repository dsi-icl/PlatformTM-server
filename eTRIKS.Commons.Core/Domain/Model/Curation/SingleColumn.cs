using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using eTRIKS.Commons.Core.Domain.Model.Base;

namespace eTRIKS.Commons.Core.Domain.Model.Curation
{
    public class SingleColumn : Identifiable<Guid> 

    {
        public List<object> colValues;
        public HeaderElements colHeader;
    }
}

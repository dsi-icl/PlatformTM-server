using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Model.Base;

namespace eTRIKS.Commons.Core.Domain.Model.Timing
{
    public abstract class TimePoint : Identifiable<int>
    {
        public string Name { get; set; }
        public int? Number { get; set; }
    }
}

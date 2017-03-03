using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Model.DesignElements;
using eTRIKS.Commons.Core.Domain.Model.Timing;

namespace eTRIKS.Commons.Core.JoinEntities
{
    public class VisitTimepoints
    {
        public Visit Visit { get; set; }
        public int VisitId { get; set; }
        public TimePoint Timepoint { get; set; }
    }
}

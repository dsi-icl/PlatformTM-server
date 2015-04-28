using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTRIKS.Commons.Core.Domain.Model.Timing
{
    public class TimeInterval
    {
        public TimePoint Start { get; set; }
        public TimePoint End { get; set; }
        public string Duration { get; set; }
    }
}

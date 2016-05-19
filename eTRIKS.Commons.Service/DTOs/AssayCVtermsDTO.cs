using eTRIKS.Commons.Core.Domain.Model.ControlledTerminology;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTRIKS.Commons.Service.DTOs
{
    public class AssayCVtermsDTO
    {
        public CVterm AssayMeasurementType { get; set; }
        public List<string> AssayPlatformTechTypes { get; set; }
        public List<string> AssayPlatformTypes { get; set; }
    }
}

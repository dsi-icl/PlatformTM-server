using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTRIKS.Commons.Core.Domain.Model
{
    public class SampleCharacteristic : Characterisitc
    {
        public Biosample Sample { get; set; }
        public int SampleId { get; set; }
    }
}

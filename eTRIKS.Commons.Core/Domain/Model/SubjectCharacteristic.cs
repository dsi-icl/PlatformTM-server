using eTRIKS.Commons.Core.Domain.Model.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTRIKS.Commons.Core.Domain.Model
{
    public class SubjectCharacteristic : Characterisitc
    {
        public Subject Subject { get; set; }
        public string SubjectId { get; set; }
    }
}

using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Core.Domain.Model.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTRIKS.Commons.Core.Domain.Model.DesignElements
{
    public class Arm : Identifiable<string>
    {
        public List<Study> Studies { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }
}

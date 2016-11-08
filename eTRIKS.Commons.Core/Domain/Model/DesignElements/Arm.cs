using eTRIKS.Commons.Core.Domain.Model.Base;
using System.Collections.Generic;

namespace eTRIKS.Commons.Core.Domain.Model.DesignElements
{
    public class Arm : Identifiable<string>
    {
        public List<Study> Studies { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }
}

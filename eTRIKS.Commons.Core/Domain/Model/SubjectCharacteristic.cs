using eTRIKS.Commons.Core.Domain.Model.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTRIKS.Commons.Core.Domain.Model
{
    public class SubjectCharacteristic : Identifiable<int>
    {
        public string Name { get; set; }
        public string DomainCode { get; set; }
        public VariableDefinition TopicVariable { get; set; }
        public List<VariableDefinition> qualifiers { get; set; }
    }
}

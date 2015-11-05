using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTRIKS.Commons.Core.Domain.Model
{
    public class ObservationNew
    {
        public ObjectOfObservation O3 { get; set; }
        public Subject subject { get; set; } //should be key value
        public VariableDefinition Qualifier { get; set; } //shuold be a key,value?
        public VariableDefinition TimeDescriptor { get; set; } //should be a key, value

    }
}

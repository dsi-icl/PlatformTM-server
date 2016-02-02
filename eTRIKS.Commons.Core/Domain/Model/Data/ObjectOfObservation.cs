using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Model.Base;
using eTRIKS.Commons.Core.Domain.Model.ControlledTerminology;

namespace eTRIKS.Commons.Core.Domain.Model.Data
{
    public class ObjectOfObservation : Identifiable<int>
    {
        public string Identifier;
        public string Name { get; set; }
        public string Domain { get; set; }
        public CVterm controlledTerm { get; set; }

        public string CVtermId { get; set; }
    }
}

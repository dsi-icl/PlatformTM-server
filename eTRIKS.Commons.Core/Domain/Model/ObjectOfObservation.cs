using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Model.Base;
using eTRIKS.Commons.Core.Domain.Model.ControlledTerminology;

namespace eTRIKS.Commons.Core.Domain.Model
{
    public class ObjectOfObservation : Identifiable<int>
    {
        private string identifier;
        private string Name { get; set; }
        private string Domain { get; set; }
        private CVterm controlledTerm { get; set; }
        private string CVtermId { get; set; }
    }
}

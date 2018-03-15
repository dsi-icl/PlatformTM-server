using System.Collections.Generic;
using PlatformTM.Core.Domain.Model.Base;

namespace PlatformTM.Core.Domain.Model.ControlledTerminology
{
    public class Dictionary : Identifiable<string>
    {
        public string Name { get; set; }
        public string Definition { get; set; }

        public string XrefId { get; set; }
        public Dbxref Xref { get; set; }

        public virtual ICollection<CVterm> Terms { get; set; }

        public Dictionary()
        {
            Terms = new List<CVterm>();
        }

    }
}
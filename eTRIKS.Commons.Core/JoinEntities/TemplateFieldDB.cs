using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Model.ControlledTerminology;
using eTRIKS.Commons.Core.Domain.Model.Templates;

namespace eTRIKS.Commons.Core.JoinEntities
{
    public class TemplateFieldDB
    {
        public DatasetTemplateField TemplateField { get; set; }
        public string TemplateFieldId { get; set; }

        public DB TermSource { get; set; }
        public string TermSourceId { get; set; }
    }
}

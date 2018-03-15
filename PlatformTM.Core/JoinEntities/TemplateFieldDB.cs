using PlatformTM.Core.Domain.Model.ControlledTerminology;
using PlatformTM.Core.Domain.Model.Templates;

namespace PlatformTM.Core.JoinEntities
{
    public class TemplateFieldDB
    {
        public DatasetTemplateField TemplateField { get; set; }
        public string TemplateFieldId { get; set; }

        public DB TermSource { get; set; }
        public string TermSourceId { get; set; }
    }
}

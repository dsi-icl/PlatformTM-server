using PlatformTM.Core.Domain.Model.Base;

namespace PlatformTM.Core.Domain.Model.DesignElements
{
    public class DesignELement : Identifiable<int>
    {
        public Study Study { get; set; }
        public int StudyId { get; set; }

        public string Name { get; set; }
    }
}

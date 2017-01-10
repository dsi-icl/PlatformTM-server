using eTRIKS.Commons.Core.Domain.Model.Base;

namespace eTRIKS.Commons.Core.Domain.Model.DesignElements
{
    public class DesignELement : Identifiable<int>
    {
        public Project Project { get; set; }
        public int ProjectId { get; set; }
        public string Name { get; set; }
    }
}

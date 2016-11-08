using eTRIKS.Commons.Core.Domain.Model.Base;
using eTRIKS.Commons.Core.Domain.Model.ControlledTerminology;
using eTRIKS.Commons.Core.Domain.Model.Templates;

namespace eTRIKS.Commons.Core.Domain.Model.Data
{
    public class ObjectOfObservation : Identifiable<int>
    {
        public string Name; //BMI
        public string Synonym { get; set; } //Body Mass Index
        public CVterm CVTerm { get; set; } //CVterm representing BMI
        public string CVtermId { get; set; }
        public string CVTermStr { get; set; } //Temporary property until OLS is setup
        public string Group { get; set; } //TODO: need to do something about this!
        public string Subgroup { get; set; }       
        public Project Project { get; set; } //OR STUDIES 
        public int ProjectId { get; set; }
        public DomainTemplate Domain { get; set; }
        public string DomainId { get; set; }
    }
}

using System.Collections.Generic;
using eTRIKS.Commons.Core.Domain.Model.Base;
using eTRIKS.Commons.Core.Domain.Model.DesignElements;

namespace eTRIKS.Commons.Core.Domain.Model
{
    public class Study : Identifiable<int>
    {
        //public string OID { get; set; }
        public string Accession { get; set; }
        public string Name { get; set; }
        public string Site { get; set; }
        public string Description { get; set; }
        public Project Project { get; set; }
        public int ProjectId { get; set; }
        //public ICollection<Activity> Activities { get; set; }
        public ICollection<Observation> Observations { get; set; }
        public ICollection<Dataset> Datasets { get; set; }
        public ICollection<Visit> Visits { get; set; }
        public ICollection<Arm> Arms { get; set; }
        public ICollection<HumanSubject> Subjects { get; set; }
    }
}
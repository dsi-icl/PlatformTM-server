using System.Collections.Generic;
using PlatformTM.Core.Domain.Model.Base;
using PlatformTM.Core.Domain.Model.DatasetModel.PDS;
using PlatformTM.Core.Domain.Model.DesignElements;
using PlatformTM.Core.JoinEntities;

namespace PlatformTM.Core.Domain.Model
{
    public class Study : Identifiable<int>
    {
        public string Accession { get; set; }
        public string Name { get; set; }
        public string Site { get; set; }
        public string Description { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; }
        
        //public ICollection<Activity> Activities { get; set; }
        //public ICollection<Observation> Observations { get; set; }

        //CONSIDER PUTTING BACK WHEN EF SUPPORTS M-2-M RELATIONSHIPS
        //public ICollection<Dataset> Datasets { get; set; }
        //public ICollection<StudyDataset> Datasets { get; set; }

        public ICollection<Assessment> Assessments { get; set; }

        public ICollection<PrimaryDataset> PrimaryDatasets { get; set; }

        public ICollection<Visit> TimeEvents { get; set; }

        public ICollection<Cohort> Cohorts { get; set; }

        public ICollection<HumanSubject> Subjects { get; set; }
    }
}
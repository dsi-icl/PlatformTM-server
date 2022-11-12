using System.Collections.Generic;
using PlatformTM.Core.Domain.Model.ControlledTerminology;
using PlatformTM.Core.Domain.Model.DatasetModel.PDS;

namespace PlatformTM.Core.Domain.Model
{
    public class Assay : Assessment
    {
        public CVterm DesignType { get; set; }
        public string DesignTypeId { get; set; }

        public CVterm TechnologyType { get; set; } //DNA Microarray, Mass Cytometry, Flow Cytometry
        public string TechnologyTypeId { get; set; }

        public CVterm TechnologyPlatform { get; set; } //Agilent, CyTOF, Luminex Multiplex Array
        public string TechnologyPlatformId { get; set; }

        public CVterm MeasurementType { get; set; } //Gene Expression, Protein Profiling ,Cytokines Profiling
        public string MeasurementTypeId { get; set; }
        

        //public ICollection<Biosample> Biosamples { get; set; }
        //public bool HasTemporalData { get; set; }

        public Assay()
        {
            Datasets = new List<PrimaryDataset>();
        }
    }
}

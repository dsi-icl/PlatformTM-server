using eTRIKS.Commons.Core.Domain.Model.ControlledTerminology;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTRIKS.Commons.Core.Domain.Model
{
    public class Assay : Activity
    {
        public CVterm DesignType { get; set; }
        public string DesignTypeId { get; set; }
        public CVterm TechnologyType { get; set; } //DNA Microarray, Mass Cytometry, Flow Cytometry
        public string TechnologyTypeId { get; set; }
        public CVterm TechnologyPlatform { get; set; } //Agilent, CyTOF, Luminex Multiplex Array
        public string TechnologyPlatformId { get; set; }
        public CVterm MeasurementType { get; set; } //Gene Expression, Protein Profiling ,Cytokines Profiling
        public string MeasurementTypeId { get; set; }
        //To be revised
        /**
         * Will drop assay factor for now since this would in most cases 
         * be attached to the sample and/or subject which in turn are always
         * linked to the assay
         **/
        //private List<string> Factors { get; set; }
        private string ObservationDomain { get; set; }
        public ICollection<Biosample> Biosamples { get; set; }

        public Assay()
        {
            //Factors = new List<string>();
            Datasets = new List<Dataset>();
        }

        //public Dataset getTargetData()
        //{
        //    return Datasets.First(d => d.DomainId.Equals("Bio"));
        //}
    }
}

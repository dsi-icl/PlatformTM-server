using System;
using System.Collections.Generic;
using PlatformTM.Core.Domain.Model.DatasetModel.PDS;

namespace PlatformTM.Core.Domain.Model.BMO
{
    public class Feature : Observable
    {
        //Need to make that more flexible
        public string Domain { get; set; }
        public string Category { get; set; }
        public string Subcategory { get; set; }

        public List<Property> FeatureProperties { get; set; }

        public List<ObservablePhenomenon> ObservablePhenomena { get; set; }

        //public int StudyId { get; set; }
        //public Study Study { get; set; }

        public int DatasetId { get; set; }
        public PrimaryDataset Dataset { get; set; }

        /**
         * This is made study-specific for now until the observed features are controlled via termIds then we 
         * can swtich to project-wide or even cross-projects observedFeatures that are referenced from the dataset r
         * rather than defined within each dataset
         */
        public Feature()
        {
            FeatureProperties = new List<Property>();
        }
    }



}


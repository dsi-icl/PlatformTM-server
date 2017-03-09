using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eTRIKS.Commons.Core.Domain.Model.Users.Queries
{
    public class AssayPanelQuery
    {
        public int AssayId { get; set; }
        //public List<ObservationQuery> FeatureQuery { get; set; }
        public List<Query> SampleQueries { get; set; }
        //public List<ObservationQuery> ObservationMeasureQuery { get; set; }

        public AssayPanelQuery()
        {
            SampleQueries = new List<Query>();
        }

    }
}

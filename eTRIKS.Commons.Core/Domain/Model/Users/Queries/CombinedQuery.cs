using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Model.Base;

namespace eTRIKS.Commons.Core.Domain.Model.Users.Queries
{
    public class CombinedQuery : Identifiable<Guid>
    {
        
        public List<ObservationQuery> SubjectCharacteristics { get; set; }
        public List<ObservationQuery> ClinicalObservations { get; set; }
        public List<ObservationQuery> AssayParameters { get; set; }

    }
}

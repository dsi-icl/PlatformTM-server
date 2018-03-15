using System;
using System.Collections.Generic;
using PlatformTM.Core.Domain.Model.Base;

namespace PlatformTM.Core.Domain.Model.Users.Queries
{
    public class CombinedQuery : Identifiable<Guid>
    {
      
        public List<Query> SubjectCharacteristics { get; set; }
        public List<ObservationQuery> ClinicalObservations { get; set; }
        public List<GroupedObservationsQuery> GroupedObservations { get; set; }
        public List<Query> DesignElements { get; set; }
        public List<AssayPanelQuery> AssayPanels { get; set; }

        public Guid UserId { get; set; }
        public int ProjectId { get; set; }
        public string Name { get; set; }
        public bool IsSavedByUser { get; set; }

        public CombinedQuery()
        {
            SubjectCharacteristics = new List<Query>();
            ClinicalObservations = new List<ObservationQuery>();
            DesignElements = new List<Query>();
            GroupedObservations = new List<GroupedObservationsQuery>();
            AssayPanels = new List<AssayPanelQuery>();
        }
    }
}

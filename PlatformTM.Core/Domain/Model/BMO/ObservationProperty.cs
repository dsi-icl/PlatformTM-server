using System;
using PlatformTM.Core.Domain.Model.DatasetModel.PDS;

namespace PlatformTM.Core.Domain.Model.BMO
{
    public class ObservationProperty : Observable
    {
       
        public Study Study { get; set; }
        public int StudyId {get;set;}

        public ObservationProperty()
        {
        }
    }
}


using System;
using PlatformTM.Core.Domain.Model.DatasetModel.SDTM;

namespace PlatformTM.Data.Repositories
{
    public class SDTMRepository : GenericMongoRepository<SdtmRow,Guid>
    {
        public SDTMRepository() : base("")
        {
            
        }
    }
}

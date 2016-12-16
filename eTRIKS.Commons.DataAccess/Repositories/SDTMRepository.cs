using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Model.DatasetModel.SDTM;

namespace eTRIKS.Commons.DataAccess.Repositories
{
    public class SDTMRepository : GenericMongoRepository<SdtmRow,Guid>
    {
        public SDTMRepository() : base("")
        {
            
        }
    }
}

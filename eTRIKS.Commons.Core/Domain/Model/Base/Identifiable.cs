using System;

namespace eTRIKS.Commons.Core.Domain.Model.Base
{
    public abstract class Identifiable<TPrimaryKey> : IEntity<TPrimaryKey>
    {
        public TPrimaryKey OID { get; set; }
        
    }
}

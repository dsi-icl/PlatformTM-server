using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace eTRIKS.Commons.Core.Domain.Interfaces
{
    public interface ICacheRepository<TEntity> where TEntity : class
    {
        TEntity GetFromCache(Expression<Func<TEntity, bool>> filter = null);
        void Save(TEntity cacheObject);
        void RemoveFromCache(Expression<Func<TEntity, bool>> filter = null);

    }
}

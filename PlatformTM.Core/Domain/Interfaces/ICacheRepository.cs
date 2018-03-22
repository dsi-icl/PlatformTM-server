using System;
using System.Linq.Expressions;

namespace PlatformTM.Core.Domain.Interfaces
{
    public interface ICacheRepository<TEntity> where TEntity : class
    {
        TEntity GetFromCache(Expression<Func<TEntity, bool>> filter = null);
        void Save(TEntity cacheObject);
        void RemoveFromCache(Expression<Func<TEntity, bool>> filter = null);

    }
}

using System;
using System.Linq.Expressions;
using MongoDB.Driver;
using PlatformTM.Core.Domain.Interfaces;

namespace PlatformTM.Data.Repositories
{
    public class CacheRepository<TEntity> : ICacheRepository<TEntity> where TEntity : class
    {
        private readonly IMongoCollection<TEntity> _collection;

        public CacheRepository(IMongoDatabase database, string collectionName)
        {
            _collection = database.GetCollection<TEntity>(collectionName);
        }

        public TEntity GetFromCache(Expression<Func<TEntity, bool>> filter = null)
        {
            return filter != null ? _collection.FindSync(filter).FirstOrDefault() : null;
        }

        public void Save(TEntity cacheObject)
        {
            _collection.InsertOne(cacheObject);
        }

        public void RemoveFromCache(Expression<Func<TEntity, bool>> filter = null)
        {
            _collection.DeleteMany(filter);
        }
    }
}

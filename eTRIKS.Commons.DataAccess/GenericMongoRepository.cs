using System.Diagnostics;
using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Core.Domain.Model.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using eTRIKS.Commons.DataAccess.Configuration;

namespace eTRIKS.Commons.DataAccess
{
    public class GenericMongoRepository <TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey>
        where TEntity : Identifiable<TPrimaryKey>, IEntity<TPrimaryKey>
    {
        private DataAccessSettings configSettings { get; set; }
        private IMongoDatabase database;
        private MongoClient mongoClient;
        public IMongoCollection<TEntity> collection;
        public string CollectionName { get; set; }
       
        public GenericMongoRepository(string collectionName)
        {
            mongoClient = new MongoClient(configSettings.mongoDBprod);
            database = mongoClient.GetDatabase(configSettings.noSQLDatabaseName);
            //collection = database.GetCollection<TEntity>("biospeak_sdtm");
            collection = database.GetCollection<TEntity>(collectionName);
        }

        public async Task<TEntity>  FindAsync(Expression<Func<TEntity, bool>> filter = null)
        {
            //await collection.InsertOneAsync(entity);
            return await collection.Find(filter).FirstOrDefaultAsync();
           
        }

        public async Task<List<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> filterExpression = null, Expression<Func<TEntity, bool>> projectionExpression = null)
        {
            if (filterExpression != null)
                return await collection
                    .Find(filterExpression)
                    .ToListAsync();
            return null;
        }

        public async Task<List<TEntity>> FindAllAsync(IList<object> filterFields = null, IList<object> projectionFields = null)
        {
            var filterDoc = new BsonDocument();
            filterDoc.AllowDuplicateNames = true;
            foreach (var filterField in filterFields)
            {
                var jsonDoc = Newtonsoft.Json.JsonConvert.SerializeObject(filterField);
                var bsonDoc = BsonSerializer.Deserialize<BsonDocument>(jsonDoc);
                filterDoc.AddRange(bsonDoc);
            }

            //Dictionary<string, object> projections = new Dictionary<string, object>();
            //projections.Add("_id", 0);
            //foreach (var item in projectionFields)
            //{
            //    projections.Add(item.Key, "$_id." + item.Key);
            //}

            return await collection.Find(filterDoc).ToListAsync();
        }


        private void TEST()
        {
            var filterBuilder = Builders<HumanSubject>.Filter;
            var filter = filterBuilder.Eq("STUDYID", "CRC305C");

            collection.Find(filter.ToBsonDocument()).ToListAsync();
        }

        public async Task<string> InsertAsync(TEntity entity)
        {
            try
            {
                await collection.InsertOneAsync(entity);
                return "RECORD(s) SUCCESSFULLY INSERTED";
            }
            catch (Exception e)
            {
                while (e.InnerException != null)
                    e = e.InnerException;
                return e.Message;
            }
            
        }

        public Task DeleteOneAsync(IList<object> filterFields = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TEntity> InsertMany(IList<TEntity> entities = null)
        {
            throw new NotImplementedException();
        }

        public async Task InsertManyAsync(IList<TEntity> entitites = null)
        {
            try
            {
                await collection.InsertManyAsync(entitites);
            }
            catch (Exception e)
            {
                while (e.InnerException != null)
                    e = e.InnerException;
                Debug.WriteLine(e.Message);
            }
        }

        public async Task DeleteManyAsync(IList<object> filterFields = null)
        {
            var filterDoc = new BsonDocument();
            filterDoc.AllowDuplicateNames = true;
            foreach (var filterField in filterFields)
            {
                var jsonDoc = Newtonsoft.Json.JsonConvert.SerializeObject(filterField);
                var bsonDoc = BsonSerializer.Deserialize<BsonDocument>(jsonDoc);
                filterDoc.AddRange(bsonDoc);
            }
            await collection.DeleteManyAsync(filterDoc);
        }

        public void DeleteMany(IList<object> filterFields = null)
        {
            var filterDoc = new BsonDocument();
            filterDoc.AllowDuplicateNames = true;
            foreach (var filterField in filterFields)
            {
                var jsonDoc = Newtonsoft.Json.JsonConvert.SerializeObject(filterField);
                var bsonDoc = BsonSerializer.Deserialize<BsonDocument>(jsonDoc);
                filterDoc.AddRange(bsonDoc);
            }
             collection.DeleteMany(filterDoc);
        }

        public void DeleteMany(Expression<Func<TEntity, bool>> filter)
        {
            throw new NotImplementedException();
        }

        public async Task<ICollection<TNewResult>> AggregateAsync<Tkey, TNewResult>(Expression<Func<TEntity, bool>> match,
                                    Expression<Func<TEntity, Tkey>> idProjector,
                                    Expression<Func<IGrouping<Tkey, TEntity>, TNewResult>> groupProjector
            )
        {

            //var projectionInfo = Aggre.TranslateGroup<Tkey, TEntity, TResult>(idProjector, groupProjector, serializer, BsonSerializer.SerializerRegistry);

            //var group = new BsonDocument("$group", projectionInfo.Document);
            return await collection
                .Aggregate()
                .Match(match)
                //.Project(projection)
                .Group(idProjector, groupProjector)
                .ToListAsync();
        }

 

        private IQueryable<TEntity> GetAll()
        {
            throw new NotImplementedException();
        }


        IQueryable<TEntity> IRepository<TEntity, TPrimaryKey>.GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<List<TEntity>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> filter = null, List<Expression<Func<TEntity, object>>> includeProperties = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, int? page = null, int? pageSize = null)
        {
            return filter != null ? collection.Find(filter).ToList() : null;
        }


        public TEntity FindSingle(Expression<Func<TEntity, bool>> filter = null, List<Expression<Func<TEntity, object>>> includeProperties = null)
        {
            return collection.FindSync(filter).Single();
        }

        public TEntity Get(TPrimaryKey key)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> GetAsync(TPrimaryKey key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TEntity> GetRecords(Expression<Func<TEntity, bool>> filter)
        {
            throw new NotImplementedException();
        }

        public TEntity Insert(TEntity entity)
        {
            
                collection.InsertOne(entity);
            return entity;


        }



        public TEntity Update(TEntity entity)
        {
            var filter = new BsonDocument("_id", Guid.Parse(entity.Id.ToString()));
            var result = collection.ReplaceOne(filter, entity);
            //throw new NotImplementedException();
            return entity;
        }

        

        public Task<int> UpdateAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public void Remove(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task<int> RemoveAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public void Remove(TPrimaryKey id)
        {
            throw new NotImplementedException();
        }


    }
}

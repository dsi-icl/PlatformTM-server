using System.Collections;
using System.Diagnostics;
using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Core.Domain.Model.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Linq.Translators;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace eTRIKS.Commons.DataAccess
{
    public class GenericMongoRepository <TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey>
        where TEntity : Identifiable<TPrimaryKey>, IEntity<TPrimaryKey>
    {
        private IMongoDatabase database;
        private MongoClient mongoClient;
        public IMongoCollection<TEntity> collection;
       
        public GenericMongoRepository()
        {
            //DataContext = dataContext;
            //DataContext.Configuration.ProxyCreationEnabled = false;
            //Entities = DataContext.Set<TEntity>();
           
            mongoClient = new MongoClient(ConfigurationManager.ConnectionStrings["MongoDBConnectionString"]
                                            .ConnectionString);
            database = mongoClient.GetDatabase(ConfigurationManager.AppSettings["NoSQLDatabaseName"]);
            //collection = database.GetCollection<TEntity>(typeof(TEntity).Name.ToLower() + "s");
            collection = database.GetCollection<TEntity>("Biospeak_clinical");
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


            return await collection.Find(filterDoc).ToListAsync();
        }


        private void TEST()
        {
            var filterBuilder = Builders<Subject>.Filter;
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

        //public List<TEntity> getGroupedNoSQLrecords(IDictionary<string, string> filterFields, IDictionary<string, string> groupingFields)
        //{
        //    return new List<TEntity>();
        //}

        //public void getNoSQLRecords(string queryString)
        //{

        //}

        //public void getDistinctNoSQLRecords(string queryString)
        //{

        //}

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
            throw new NotImplementedException();
        }




        public TEntity FindSingle(Expression<Func<TEntity, bool>> filter = null, List<Expression<Func<TEntity, object>>> includeProperties = null)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }



        public TEntity Update(TEntity entity)
        {
            throw new NotImplementedException();
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

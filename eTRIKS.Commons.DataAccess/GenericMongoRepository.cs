using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.Core.Domain.Model.Base;
using MongoDB.Bson;
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
            collection = database.GetCollection<TEntity>(typeof(TEntity).Name.ToLower() + "s");
            //collection = database.GetCollection<TEntity>("BioSpeak_Data");
        }

        public async Task<TEntity>  FindAsync(Expression<Func<TEntity, bool>> filter = null)
        {
            //await collection.InsertOneAsync(entity);
            return await collection.Find(filter).FirstOrDefaultAsync();
           
        }

        public async Task<List<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> filter = null)
        {
            //BsonDocument filter2;
            //if (filter == null)
            //    filter2 = new BsonDocument();
            //filter.ToBsonDocument();
            return await collection.Find(filter).ToListAsync();
        }

        public async Task InsertAsync(TEntity entity)
        {
            await collection.InsertOneAsync(entity);
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

        public List<TEntity> getGroupedNoSQLrecords(IDictionary<string, string> filterFields, IDictionary<string, string> groupingFields)
        {
            return new List<TEntity>();
        }

        public void getNoSQLRecords(string queryString)
        {

        }

        public void getDistinctNoSQLRecords(string queryString)
        {

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

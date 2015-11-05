using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.Core.Domain.Model.Base;


namespace eTRIKS.Commons.DataAccess
{
    public class GenericRepository<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey>
        where TEntity : Identifiable<TPrimaryKey>, IEntity<TPrimaryKey>
    {
        //TODO: IS THIS REALLY NEEDED?
        //THIS AS IT IS RIGHT NOW WITH NO DEPENDENCE ON EF MAKES THIS REPOSITORY IMPLEMENTATION
        //AN INDEPENDENT IMPLMENETATION THAT COULD BE USED FOR TESTING AS WELL IF A TESTDATACONTEXT IS PASSED
        //TO DATACONTEXT INSTEAD OF ETRIKSDATACONTEXT
        // IF THIS GETS CHANGED TO DbContext DataContext instead of IDataContext DataContext
        // THEN this means we will have to create ANOTHER Repository Implementation for TESTING instead of using
        // this one and only passnig it a different context (or a different DBset)
        protected DbContext DataContext { get; set; }
        protected DbSet<TEntity> Entities { get; set; }
        public GenericRepository(DbContext dataContext)
        {
            DataContext = dataContext;
            DataContext.Configuration.ProxyCreationEnabled = false;
            Entities = DataContext.Set<TEntity>();
            
        }
        public GenericRepository(IDbSet<TEntity> entities)
        {
            Entities = (DbSet<TEntity>)entities;
            //dataContext = entities
        }

        public IQueryable<TEntity> GetAll()
        {
            return Entities;
        }
        public TEntity Get(TPrimaryKey key)
        {
            return Entities.Find(key);
        }

        public IEnumerable<TEntity> GetRecords(Expression<Func<TEntity, bool>> filter)
        {
            IQueryable<TEntity> dbQuery = Entities;
            return dbQuery.Where(filter);
        }


        public IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> filter = null, 
                                    List<Expression<Func<TEntity, object>>> includeProperties = null,
                                    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                    int? page = null,
                                    int? pageSize = null)
        {
           
            IQueryable<TEntity> query = Entities;

            if (includeProperties != null)
                includeProperties.ForEach(i => 
                    query = query.Include<TEntity, object>(i));

            if (filter != null)
                query = query.Where(filter);

            if (orderBy != null)
                query = orderBy(query);
 
            if (page != null && pageSize != null)
                query = query
                    .Skip((page.Value - 1)*pageSize.Value)
                    .Take(pageSize.Value);

            return query.ToList<TEntity>();
        }



        public TEntity FindSingle(Expression<Func<TEntity, bool>> filter = null,
                                    List<Expression<Func<TEntity, object>>> includeProperties = null)
        {
            return FindAll(filter, includeProperties).SingleOrDefault();
        }

        public TEntity FindAll(TPrimaryKey key)
        {
            return Entities.Find(key);
        }

        

        public TEntity Insert(TEntity entity)
        {
            return Entities.Add(entity);
        }

        public TEntity Update(TEntity entity)
        {
            //Entities.Attach(entity);
            DataContext.Entry(entity).State = EntityState.Modified;
            return entity;
        }

        public void Remove(TEntity entity)
        {
            Entities.Attach(entity);
            Entities.Remove(entity);
        }

        public void Remove(TPrimaryKey id)
        {
            throw new NotImplementedException();
        }




        public System.Threading.Tasks.Task<List<TEntity>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<List<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> filter)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> filter)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<TEntity> GetAsync(TPrimaryKey key)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<string> InsertAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<int> UpdateAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<int> RemoveAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<ICollection<TNewResult>> AggregateAsync<Tkey, TNewResult>(Expression<Func<TEntity, bool>> match, Expression<Func<TEntity, Tkey>> idProjector, Expression<Func<IGrouping<Tkey, TEntity>, TNewResult>> groupProjector)
        {
            throw new NotImplementedException();
        }
    }
}

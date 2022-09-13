using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PlatformTM.Core.Domain.Interfaces;
using PlatformTM.Core.Domain.Model.Base;

namespace PlatformTM.Data.Repositories
{
    public class GenericRepository<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey>
        where TEntity : Identifiable<TPrimaryKey>, IEntity<TPrimaryKey>
    {
        protected DbContext DataContext { get; set; }
        protected DbSet<TEntity> Entities { get; set; }
        public GenericRepository(DbContext dataContext)
        {
            DataContext = dataContext;
            //DataContext.Configuration.ProxyCreationEnabled = false;
            Entities = DataContext.Set<TEntity>(); 
        }
        public GenericRepository(DbSet<TEntity> entities)
        {
            Entities = (DbSet<TEntity>)entities;
            //dataContext = entities
        }
      
        public List<object> FindObservations(Expression<Func<TEntity, bool>> filterExpression = null, Expression<Func<TEntity, object>> projectionExpression = null)
        {
            throw new NotImplementedException();
        }
        


        public TEntity Get(TPrimaryKey key)
        {
            return Entities.Find(key);
        }

        public IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> filter = null, 
                                    //List<Expression<Func<TEntity, object>>> includeProperties = null,
                                    List<string> includeProperties = null,
                                    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                    int? page = null,
                                    int? pageSize = null)
        {
           
            IQueryable<TEntity>[] query = {Entities};

            includeProperties?.ForEach(i =>
                query[0] = query[0].Include(i));

            if (filter != null)
                query[0] = query[0].Where(filter);

            if (orderBy != null)
                query[0] = orderBy(query[0]);
 
            if (page != null && pageSize != null)
                query[0] = query[0]
                    .Skip((page.Value - 1)*pageSize.Value)
                    .Take(pageSize.Value);

            return query[0].ToList<TEntity>();
        }

        public Task<List<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> filter = null, List<string> includeProperties = null, Expression<Func<TEntity, bool>> projectionExpression = null)
        {
            IQueryable<TEntity>[] query = { Entities };

            includeProperties?.ForEach(i =>
                query[0] = query[0].Include(i));

            if (filter != null)
                query[0] = query[0].Where(filter);
            

            return query[0].ToListAsync<TEntity>();
        }

        public Task<List<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> filterExpression = null, Expression<Func<TEntity, bool>> projectionExpression = null)
        {
            throw new NotImplementedException();
        }

        public Task<List<TEntity>> FindAllAsync(IList<object> filterFields = null, IList<object> projectionFields = null)
        {
            throw new NotImplementedException();
        }


        public TEntity FindSingle(Expression<Func<TEntity, bool>> filter = null,
                                    List<string> includeProperties = null)
        {
            return FindAll(filter, includeProperties).SingleOrDefault();
        }

        public TEntity FindAll(TPrimaryKey key)
        {
            return Entities.Find(key);
        }

        

        public TEntity Insert(TEntity entity)
        {
            return Entities.Add(entity).Entity;
        }

        public void InsertMany(IList<TEntity> entities = null)
        {
            Entities.AddRange(entities);
        }

        public Task InsertManyAsync(IList<TEntity> entitites = null)
        {
            throw new NotImplementedException();
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

        public Task DeleteManyAsync(IList<object> filterFields = null)
        {
            throw new NotImplementedException();
        }

        public Task DeleteOneAsync(IList<object> filterFields = null)
        {
            throw new NotImplementedException();
        }

        public void DeleteMany(IList<object> filterFields = null)
        {
            throw new NotImplementedException();
        }

        public void DeleteMany(Expression<Func<TEntity, bool>> filter)
        {
            var entitiesToDelete = FindAll(filter);
            Entities.RemoveRange(entitiesToDelete);
        }


        public System.Threading.Tasks.Task<List<TEntity>> GetAllAsync()
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

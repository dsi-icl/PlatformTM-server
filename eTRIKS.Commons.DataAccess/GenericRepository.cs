using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using eTRIKS.Commons.Core.Domain.Model.Base;
using eTRIKS.Commons.Core.Interfaces;

namespace eTRIKS.Commons.DataAccess
{
    public class GenericRepository<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey>
        where TEntity : Identifiable<TPrimaryKey>, IEntity<TPrimaryKey>
    {
        protected IDataContext DataContext { get; set; }
        protected IDbSet<TEntity> Entities { get; set; }
        public GenericRepository(IDataContext dataContext)
        {
            DataContext = dataContext;
            Entities = DataContext.Set<TEntity>();
        }

        public IQueryable<TEntity> GetAll()
        {
            return Entities;
        }

        public List<TEntity> GetAllList()
        {
            return Entities.ToList();
        }

        public List<TEntity> GetAllList(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Where(predicate).ToList();
        }

        public IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null, 
                                    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                    List<Expression<Func<TEntity, object>>> includeProperties = null,
                                    int? page = null,
                                    int? pageSize = null)
        {
           
            IQueryable<TEntity> query = Entities;
     
            if (includeProperties != null)
                includeProperties.ForEach(i => query.Include(i));
 
            if (filter != null)
                query = query.Where(filter);
 
            if (orderBy != null)
                query = orderBy(query);
 
            if (page != null && pageSize != null)
                query = query
                    .Skip((page.Value - 1)*pageSize.Value)
                    .Take(pageSize.Value);
 
            return query.ToList();
        }

        public TEntity GetById(TPrimaryKey key)
        {
            return Entities.Find(key);
        }

        public TEntity Insert(TEntity entity)
        {
            return Entities.Add(entity);
        }

        public TEntity Update(TEntity entity)
        {
            Entities.Attach(entity);
            DataContext.Entry(entity).State = EntityState.Modified;
            return entity;
        }

        public void Delete(TEntity entity)
        {
            Entities.Attach(entity);
            Entities.Remove(entity);
        }

        public void Delete(TPrimaryKey id)
        {
            throw new NotImplementedException();
        }

        public int Count()
        {
            throw new NotImplementedException();
        }

        public int Count(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }
    }
}

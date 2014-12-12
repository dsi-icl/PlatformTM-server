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
        protected IDbSet<TEntity> Entities { get; set; }
        public GenericRepository(DbContext dataContext)
        {
            DataContext = dataContext;
            DataContext.Configuration.ProxyCreationEnabled = false;
            Entities = DataContext.Set<TEntity>();
        }
        public GenericRepository(IDbSet<TEntity> entities)
        {
            Entities = entities;
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

        public  TEntity GetList(Func<TEntity, bool> where,
                 params Expression<Func<TEntity, object>>[] navigationProperties)
        {
            
            TEntity data = null;
            //using (var context = new DataContext())
            //{
           //IQueryable<TEntity> dbQuery = DataContext.Set<TEntity>();
            IQueryable<TEntity> query = Entities;
                //Apply eager loading
                foreach (Expression<Func<TEntity, object>> navigationProperty in navigationProperties)
                    query = query.Include<TEntity, object>(navigationProperty);
                try
                {
                    data = query
                        .AsNoTracking()
                        .FirstOrDefault(where);
                }
                catch (Exception e) { }
            //}

            return data;
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

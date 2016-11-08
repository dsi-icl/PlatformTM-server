using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace eTRIKS.Commons.Core.Domain.Interfaces
{
    public interface IRepository<TEntity, in TPrimaryKey> where TEntity :class
    {

       #region Select/Get/Query

        /// <summary>
        ///
        /// </summary>
        /// <returns>IQueryable to be used to select entities from database</returns>
        IQueryable<TEntity> GetAll();

        /// <summary>
        /// Used to get all entities.
        /// </summary>
        /// <returns>List of all entities</returns>
        //List<TEntity> GetAllList();
        Task<List<TEntity>> GetAllAsync();

        /// <summary>
        /// Gets an entity with given primary key.
        /// </summary>
        /// <param name="key">Primary key of the entity to get</param>
        /// <returns>Entity</returns>
        TEntity Get(TPrimaryKey key);

        Task<TEntity> GetAsync(TPrimaryKey key);

        IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> filter = null,
                                    List<Expression<Func<TEntity, object>>> includeProperties = null,
                                    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                    int? page = null,
                                    int? pageSize = null);

        Task<List<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> filterExpression = null, Expression<Func<TEntity, bool>> projectionExpression = null);

        Task<List<TEntity>> FindAllAsync(IList<object> filterFields = null, IList<object> projectionFields = null);


        TEntity FindSingle(Expression<Func<TEntity, bool>> filter = null,
                           List<Expression<Func<TEntity, object>>> includeProperties = null);
        Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> filterExpression = null);



        IEnumerable<TEntity> GetRecords(Expression<Func<TEntity, bool>> filter);

        #endregion

        #region Insert

        /// <summary>
        /// Inserts a new entity.
        /// </summary>
        /// <param name="entity">Entity</param>
        TEntity Insert(TEntity entity);
        Task<string> InsertAsync(TEntity entity);
        IEnumerable<TEntity> InsertMany(IList<TEntity> entities = null);
        Task InsertManyAsync(IList<TEntity> entitites = null);

        #endregion

        #region Update

        /// <summary>
        /// Modify an existing entity.
        /// </summary>
        /// <param name="entity">Entity</param>
        TEntity Update(TEntity entity);
        Task<int> UpdateAsync(TEntity entity);

        #endregion

        #region Delete

        /// <summary>
        /// Deletes an entity.
        /// </summary>
        /// <param name="entity">Entity to be deleted</param>
        void Remove(TEntity entity);
        Task<int> RemoveAsync(TEntity entity);

        /// <summary>
        /// Deletes an entity by primary key.
        /// </summary>
        /// <param name="id">Primary key of the entity</param>
        void Remove(TPrimaryKey id);
        Task DeleteManyAsync(IList<object> filterFields = null);
        Task DeleteOneAsync(IList<object> filterFields = null);
        void DeleteMany(IList<object> filterFields = null);
        void DeleteMany(Expression<Func<TEntity, bool>> filter);
        
        #endregion

        #region Aggregates



        Task<ICollection<TNewResult>> AggregateAsync<Tkey, TNewResult>(Expression<Func<TEntity, bool>> match,
                                    Expression<Func<TEntity, Tkey>> idProjector,
                                    Expression<Func<IGrouping<Tkey, TEntity>, TNewResult>> groupProjector
            /*,Expression<Func<TEntity, TResult>> projection*/);

        #endregion
    }
}

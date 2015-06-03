using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace eTRIKS.Commons.Core.Domain.Interfaces
{
    public interface IRepository<TEntity, in TPrimaryKey>
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
        /// Used to get all entities based on given <see cref="predicate"/>.
        /// </summary>
        /// <param name="predicate">A condition to filter entities</param>
        /// <returns>List of all entities</returns>
        //List<TEntity> GetAllList(Expression<Func<TEntity, bool>> predicate);

        //TEntity GetList(Func<TEntity, bool> where, params Expression<Func<TEntity, object>>[] navigationProperties);

        IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> filter = null,
                                    List<Expression<Func<TEntity, object>>> includeProperties = null,
                                    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                    int? page = null,
                                    int? pageSize = null);
        Task<List<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> filter = null);

        TEntity FindSingle(Expression<Func<TEntity, bool>> filter = null,
                           List<Expression<Func<TEntity, object>>> includeProperties = null);
        Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> filter);

        /// <summary>
        /// Gets an entity with given primary key.
        /// </summary>
        /// <param name="key">Primary key of the entity to get</param>
        /// <returns>Entity</returns>
        TEntity Get(TPrimaryKey key);

        Task<TEntity> GetAsync(TPrimaryKey key);

        IEnumerable<TEntity> GetRecords(Expression<Func<TEntity, bool>> filter);

        #endregion

        #region Insert

        /// <summary>
        /// Inserts a new entity.
        /// </summary>
        /// <param name="entity">Entity</param>
        TEntity Insert(TEntity entity);
        Task<string> InsertAsync(TEntity entity);

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


        #endregion

        #region Aggregates



        Task<ICollection<TNewResult>> AggregateAsync<Tkey, TNewResult>(Expression<Func<TEntity, bool>> match,
                                    Expression<Func<TEntity, Tkey>> idProjector,
                                    Expression<Func<IGrouping<Tkey, TEntity>, TNewResult>> groupProjector
            /*,Expression<Func<TEntity, TResult>> projection*/);

        #endregion
    }
}

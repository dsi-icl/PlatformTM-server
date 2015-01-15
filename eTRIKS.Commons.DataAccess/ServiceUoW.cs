using System;
using System.Data.Entity;
using System.Linq;
using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.Core.Domain.Model.Base;
using System.Collections.Generic;

namespace eTRIKS.Commons.DataAccess {
    public class ServiceUoW : DbContext, IServiceUoW {
        //private readonly IDataContext _dataContext;

        private readonly Dictionary<Type, object> _repositories;
        private bool _disposed;

        public ServiceUoW() {
            //_dataContext = context;
            _repositories = new Dictionary<Type, object>();
            _disposed = false;
        }

        public IRepository<TEntity, TPrimaryKey> GetRepository<TEntity, TPrimaryKey>() 
            where TEntity : Identifiable<TPrimaryKey>, IEntity<TPrimaryKey> {

            // Checks if the Dictionary Key contains the Model class
            if (_repositories.Keys.Contains(typeof(TEntity))){
                // Return the repository for that Model class
                return _repositories[typeof(TEntity)] as IRepository<TEntity,TPrimaryKey>;
            }

            // If the repository for that Model class doesn't exist, create it
           // var repository = new GenericRepository<TEntity,TPrimaryKey>(_dataContext);
            var repository = new GenericRepository<TEntity, TPrimaryKey>(base.Set<TEntity>());

            // Add it to the dictionary
            _repositories.Add(typeof(TEntity), repository);

            return repository;
        }

    
        public string Save()
        {
            try
            {
                base.SaveChanges();
                return "CREATED";
            }
            catch (Exception e)
            {
                while (e.InnerException != null)
                    e = e.InnerException;
                return e.Message;
            }
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (this._disposed) return;
            if (disposing)
                base.Dispose();
            this._disposed = true;
        }
    }
}

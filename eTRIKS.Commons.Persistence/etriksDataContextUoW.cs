using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.Core.Domain.Model.Base;
using eTRIKS.Commons.DataAccess;
using eTRIKS.Commons.Persistence.Mapping;

namespace eTRIKS.Commons.Persistence {
    public class etriksDataContextUoW : DbContext, IServiceUoW {
        //private readonly IDataContext _dataContext;

        private readonly Dictionary<Type, object> _repositories;
        private bool _disposed;

        public etriksDataContextUoW() : base("name=eTRIKScontext_MySQL"){
            //_dataContext = context;
            Database.SetInitializer<etriksDataContextUoW>(null);
            
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
          
        public void Save() {
            base.SaveChanges();
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

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new DomainDatasetMap());
            modelBuilder.Configurations.Add(new DomainVariableMap());
            modelBuilder.Configurations.Add(new DatasetMap());
            modelBuilder.Configurations.Add(new ActivityMap());
            modelBuilder.Configurations.Add(new CVtermMap());
            modelBuilder.Configurations.Add(new DbMap());
            modelBuilder.Configurations.Add(new DBxrefMap());
            modelBuilder.Configurations.Add(new DerivedVariablePropertiesMap());
            modelBuilder.Configurations.Add(new DictionaryMap());
            modelBuilder.Configurations.Add(new StudyMap());
            modelBuilder.Configurations.Add(new VariableDefMap());
            modelBuilder.Configurations.Add(new VariableRefMap());
        }
    }
}

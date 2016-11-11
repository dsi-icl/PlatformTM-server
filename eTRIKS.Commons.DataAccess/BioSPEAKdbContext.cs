using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.Core.Domain.Model.Base;
using eTRIKS.Commons.Core.Application.AccountManagement;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Core.Domain.Model.Users.Datasets;
using System.Threading.Tasks;
using MySQL.Data.EntityFrameworkCore.Extensions;
using eTRIKS.Commons.DataAccess.Helpers;
using eTRIKS.Commons.Persistence.Mapping;

namespace eTRIKS.Commons.DataAccess
{

    //[DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class BioSPEAKdbContext : DbContext, IServiceUoW
    {
        //private readonly IDataContext _dataContext;

        private readonly Dictionary<Type, object> _repositories;
        private IUserRepository userRepository;
        private IUserAccountRepository _userAccountRepository;
        private bool _disposed;


        public BioSPEAKdbContext(DbContextOptions<BioSPEAKdbContext> options) : base(options){
            _repositories = new Dictionary<Type, object>();
            _disposed = false;
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //    => optionsBuilder
        //        .UseMySQL(@"Server=localhost;database=ef;uid=root;pwd=19931101;");

        //public BioSPEAKdbContext() : base("name=eTRIKScontext_MySQL")
        //{
        //    //_dataContext = context;

        //    Database.SetInitializer<BioSPEAKdbContext>(null);

        //    DbConfiguration.SetConfiguration(new MySqlEFConfiguration());
        //    Configuration.ProxyCreationEnabled = true;
        //    Configuration.LazyLoadingEnabled = true;

        //    _repositories = new Dictionary<Type, object>();

        //    this.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
        //    _disposed = false;
        //}

        public IUserRepository GetUserRepository()
        {
            return userRepository as IUserRepository ?? (userRepository = new UserRepository(this)) as IUserRepository;
        }

        public IUserAccountRepository GetUserAccountRepository()
        {
            return _userAccountRepository as IUserAccountRepository ?? (_userAccountRepository = new UserAccountRepository(this)) as IUserAccountRepository;
        }

        public IRepository<TEntity, TPrimaryKey> GetRepository<TEntity, TPrimaryKey>()
            where TEntity : Identifiable<TPrimaryKey>, IEntity<TPrimaryKey>
        {

            // Checks if the Dictionary Key contains the Model class
            if (_repositories.Keys.Contains(typeof(TEntity)))
            {
                // Return the repository for that Model class
                return _repositories[typeof(TEntity)] as IRepository<TEntity, TPrimaryKey>;
            }

            // If the repository for that Model class doesn't exist, create it
            if (typeof(TEntity).Name.Equals("SubjectObservation"))
            {
                var MongoRepository = new GenericMongoRepository<TEntity, TPrimaryKey>("Biospeak_clinical");
                _repositories.Add(typeof(TEntity), MongoRepository);
                return MongoRepository;
            }

            if (typeof(TEntity).Name.Equals("MongoDocument"))
            {
                var MongoRepository = new GenericMongoRepository<TEntity, TPrimaryKey>("Biospeak_clinical");
                _repositories.Add(typeof(TEntity), MongoRepository);
                return MongoRepository;
            }
            if (typeof(TEntity).Name.Equals("SdtmRow") || typeof(TEntity)==(typeof(PlatformAnnotation)))
            {
                var MongoRepository = new GenericMongoRepository<TEntity, TPrimaryKey>("Biospeak_clinical");
                _repositories.Add(typeof(TEntity), MongoRepository);
                return MongoRepository;
            }
            if (typeof(TEntity) == (typeof(UserDataset)))
            {
                var MongoRepository = new GenericMongoRepository<TEntity, TPrimaryKey>("userDatasets");
                _repositories.Add(typeof(TEntity), MongoRepository);
                return MongoRepository;
            }

            var repository = new GenericRepository<TEntity, TPrimaryKey>(this);

            // Add it to the dictionary
            _repositories.Add(typeof(TEntity), repository);

            return repository;
        }

        public string Save()
        {
            try
            {
                base.SaveChanges();
                // tran.Complete();
                return "CREATED";
            }
            catch (Exception e)
            {
                //tran.Dispose();
                while (e.InnerException != null)
                    e = e.InnerException;
                return e.Message;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this._disposed) return;
            if (disposing)
                base.Dispose();
            this._disposed = true;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.AddConfiguration(new DomainTemplateMap());
            //modelBuilder.AddConfiguration(new DomainVariableTemplateMap());
            //modelBuilder.AddConfiguration(new DatasetMap());
            //modelBuilder.AddConfiguration(new ActivityMap());
            //modelBuilder.AddConfiguration(new CVtermMap());
            //modelBuilder.AddConfiguration(new DbMap());
            //modelBuilder.AddConfiguration(new DBxrefMap());
            //modelBuilder.AddConfiguration(new DerivedVariablePropertiesMap());
            //modelBuilder.AddConfiguration(new DictionaryMap());
            //modelBuilder.AddConfiguration(new StudyMap());
            //modelBuilder.AddConfiguration(new VariableDefMap());
            //modelBuilder.AddConfiguration(new VariableRefMap());
            //modelBuilder.AddConfiguration(new ObservationMap());
            //modelBuilder.AddConfiguration(new ProjectMap());
            //modelBuilder.AddConfiguration(new DataFileMap());
            //modelBuilder.AddConfiguration(new VisitMap());
            //modelBuilder.AddConfiguration(new SubjectMap());
            //modelBuilder.AddConfiguration(new BioSampleMap());
            //modelBuilder.AddConfiguration(new CharacterisitcMap());
            //modelBuilder.AddConfiguration(new CharacteristicObjectMap());
            //modelBuilder.AddConfiguration(new ArmMap());
            //modelBuilder.AddConfiguration(new UserMap());
            //modelBuilder.AddConfiguration(new AccountMap());
            //modelBuilder.AddConfiguration(new ClaimMap());
           
        }

        public Task<int> SaveChangesAsync()
        {
            throw new NotImplementedException();
        }
    }
}

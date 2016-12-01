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
using eTRIKS.Commons.DataAccess.Configuration;
using eTRIKS.Commons.DataAccess.EntityConfigurations;
using eTRIKS.Commons.DataAccess.Helpers;
using eTRIKS.Commons.DataAccess.Repositories;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace eTRIKS.Commons.DataAccess
{
    public class BioSPEAKdbContext : DbContext, IServiceUoW
    {
        private readonly IOptions<DataAccessSettings> _dbsettings;
        //private readonly IDataContext _dataContext;

        private readonly Dictionary<Type, object> _repositories;
        private IUserRepository userRepository;
        private IUserAccountRepository _userAccountRepository;
        private bool _disposed;


        public BioSPEAKdbContext(DbContextOptions<BioSPEAKdbContext> options, IOptions<DataAccessSettings> settings) : base(options){
            _dbsettings = settings;
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

            if (typeof(TEntity).Name.Equals("SdtmRow") || typeof(TEntity) == (typeof(PlatformAnnotation)))
            {
                var mongoClient = new MongoClient(_dbsettings.Value.MongoDBconnection);
                var mongodb = mongoClient.GetDatabase(_dbsettings.Value.noSQLDatabaseName);

                var MongoRepository = new GenericMongoRepository<TEntity, TPrimaryKey>(mongodb,"Biospeak_clinical");
                _repositories.Add(typeof(TEntity), MongoRepository);
                return MongoRepository;
            }
            if (typeof(TEntity) == (typeof(UserDataset)))
            {
                var mongoClient = new MongoClient(_dbsettings.Value.MongoDBconnection);
                var mongodb = mongoClient.GetDatabase(_dbsettings.Value.noSQLDatabaseName);
                var MongoRepository = new GenericMongoRepository<TEntity, TPrimaryKey>(mongodb,"userDatasets");
                _repositories.Add(typeof(TEntity), MongoRepository);
                return MongoRepository;
            }

            var repository = new GenericRepository<TEntity, TPrimaryKey>(this);
            _repositories.Add(typeof(TEntity), repository);

            return repository;
        }

        public void Register<TEntity, TPrimaryKey>(IRepository<TEntity, TPrimaryKey> repository) where TEntity : Identifiable<TPrimaryKey>, IEntity<TPrimaryKey>
        {
            _repositories.Add(typeof(TEntity), repository);
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

        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
                base.Dispose();
            _disposed = true;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.AddConfiguration(new AccountConfig());
            modelBuilder.AddConfiguration(new ActivityConfig());
            modelBuilder.AddConfiguration(new ArmConfig());
            modelBuilder.AddConfiguration(new AssayConfig());

            modelBuilder.AddConfiguration(new BioSampleConfig());
            modelBuilder.AddConfiguration(new CharacterisitcConfig());
            modelBuilder.AddConfiguration(new CharacteristicObjectConfig());
            modelBuilder.AddConfiguration(new ClaimConfig());



            modelBuilder.AddConfiguration(new DomainTemplateConfig());
            modelBuilder.AddConfiguration(new DomainTemplateVariableConfig());
            modelBuilder.AddConfiguration(new CVtermConfig());
            modelBuilder.AddConfiguration(new DbConfig());
            modelBuilder.AddConfiguration(new DBxrefConfig());
            modelBuilder.AddConfiguration(new DictionaryConfig());

            
            modelBuilder.AddConfiguration(new DatafileConfig());
            modelBuilder.AddConfiguration(new DatasetConfig());
            modelBuilder.AddConfiguration(new DatasetDatafileConfig());

            modelBuilder.AddConfiguration(new ObservationConfig());
            modelBuilder.AddConfiguration(new ObservationQualifiersConfig());
            modelBuilder.AddConfiguration(new ObservationSynonymConfig());
            modelBuilder.AddConfiguration(new ObservationTimingsConfig());

            modelBuilder.AddConfiguration(new ProjectConfig());
            modelBuilder.AddConfiguration(new ProjectUserConfig());

            modelBuilder.AddConfiguration(new StudyConfig());
            modelBuilder.AddConfiguration(new StudyDatasetConfig());
            modelBuilder.AddConfiguration(new StudyArmConfig());

            modelBuilder.AddConfiguration(new SubjectConfig());

            modelBuilder.AddConfiguration(new TimePointConfig());

            modelBuilder.AddConfiguration(new UserConfig());


            modelBuilder.AddConfiguration(new VariableDefConfig());
            modelBuilder.AddConfiguration(new VariableRefConfig());

            modelBuilder.AddConfiguration(new VisitConfig());

           

            


            
           
           

        }

        public Task<int> SaveChangesAsync()
        {
            throw new NotImplementedException();
        }
    }
}

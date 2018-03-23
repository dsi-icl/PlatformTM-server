using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MySql.Data.MySqlClient;
using PlatformTM.Core.Application.AccountManagement;
using PlatformTM.Core.Domain.Interfaces;
using PlatformTM.Core.Domain.Model;
using PlatformTM.Core.Domain.Model.Base;
using PlatformTM.Core.Domain.Model.ControlledTerminology;
using PlatformTM.Core.Domain.Model.DatasetModel;
using PlatformTM.Core.Domain.Model.DesignElements;
using PlatformTM.Core.Domain.Model.Templates;
using PlatformTM.Core.Domain.Model.Timing;
using PlatformTM.Core.Domain.Model.Users;
using PlatformTM.Core.Domain.Model.Users.Datasets;
using PlatformTM.Core.Domain.Model.Users.Queries;
using PlatformTM.Core.JoinEntities;
using PlatformTM.Data.Configuration;
using PlatformTM.Data.EntityConfigurations;
using PlatformTM.Data.Extensions;
using PlatformTM.Data.Repositories;
using Observation = PlatformTM.Core.Domain.Model.ObservationModel.Observation;

namespace PlatformTM.Data
{
    public class PlatformTMdbContext : DbContext, IServiceUoW
    {
        private readonly IOptions<DataAccessSettings> _dbsettings;
        //private readonly IDataContext _dataContext;

        private readonly Dictionary<Type, object> _repositories;
        private readonly Dictionary<Type, object> _cacheRepositories;
        private IUserRepository userRepository;
        private IUserAccountRepository _userAccountRepository;
        private MongoClient _mongoClient;
        private IMongoDatabase _mongodb;
        private bool _disposed;


        public PlatformTMdbContext(DbContextOptions<PlatformTMdbContext> options, IOptions<DataAccessSettings> settings) : base(options){
            _dbsettings = settings;
            _repositories = new Dictionary<Type, object>();
            _cacheRepositories = new Dictionary<Type,object>();
            _mongoClient = new MongoClient(_dbsettings.Value.MongoDBconnection);
            _mongodb = _mongoClient.GetDatabase(_dbsettings.Value.noSQLDatabaseName);
            _disposed = false;
        }


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
                Debug.WriteLine("Retrieving repository for ", typeof(TEntity).Name);
                return _repositories[typeof(TEntity)] as IRepository<TEntity, TPrimaryKey>;
            }

            if (typeof(TEntity).Name.Equals("SdtmRow") || typeof(TEntity) == (typeof(PlatformAnnotation)))
            {
                //var mongoClient = new MongoClient(_dbsettings.Value.MongoDBconnection);
                //var mongodb = mongoClient.GetDatabase(_dbsettings.Value.noSQLDatabaseName);

                var mongoRepository = new GenericMongoRepository<TEntity, TPrimaryKey>(_mongodb,"Biospeak_clinical");
                _repositories.Add(typeof(TEntity), mongoRepository);
                return mongoRepository;
            }
            if (typeof(TEntity) == typeof(UserDataset))
            {
                //var mongoClient = new MongoClient(_dbsettings.Value.MongoDBconnection);
                //var mongodb = mongoClient.GetDatabase(_dbsettings.Value.noSQLDatabaseName);
                var mongoRepository = new GenericMongoRepository<TEntity, TPrimaryKey>(_mongodb,"userDatasets");
                _repositories.Add(typeof(TEntity), mongoRepository);
                return mongoRepository;
            }
            if (typeof(TEntity) == typeof(CombinedQuery))
            {
               
                var mongoRepository = new GenericMongoRepository<TEntity, TPrimaryKey>(_mongodb, "userQueries");
                _repositories.Add(typeof(TEntity), mongoRepository);
                return mongoRepository;
            }
            if (typeof(TEntity) == typeof(Observation) )
            {
                var mongoClient = new MongoClient(_dbsettings.Value.MongoDBconnection);
                var mongodb = mongoClient.GetDatabase(_dbsettings.Value.noSQLDatabaseName);
                var MongoRepository = new GenericMongoRepository<TEntity, TPrimaryKey>(mongodb, "assayObservation");
                _repositories.Add(typeof(TEntity), MongoRepository);
                return MongoRepository;
            }
            var repository = new GenericRepository<TEntity, TPrimaryKey>(this);
            _repositories.Add(typeof(TEntity), repository);

            return repository;
        }

        public ICacheRepository<TEntity> GetCacheRepository<TEntity>() where TEntity : class
        {
            if (_cacheRepositories.Keys.Contains(typeof(TEntity)))
            {
                Debug.WriteLine("Retrieving repository for ", typeof(TEntity).Name);
                return _cacheRepositories[typeof(TEntity)] as ICacheRepository<TEntity>;
            }

            var mongoClient = new MongoClient(_dbsettings.Value.MongoDBconnection);
            var mongodb = mongoClient.GetDatabase(_dbsettings.Value.noSQLDatabaseName);

            var cacheRepository = new CacheRepository<TEntity>(mongodb, "Biospeak_cache");
            _cacheRepositories.Add(typeof(TEntity), cacheRepository);
            return cacheRepository;
        }

        public string Save()
        {
            try
            {
                var ret = base.SaveChanges();
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

            modelBuilder.AddConfiguration<UserAccount>(new AccountConfig());
            modelBuilder.AddConfiguration<Core.Domain.Model.Activity>(new ActivityConfig());
            modelBuilder.AddConfiguration<Arm>(new ArmConfig());
            modelBuilder.AddConfiguration<Assay>(new AssayConfig());

            modelBuilder.AddConfiguration<Biosample>(new BioSampleConfig());
            modelBuilder.AddConfiguration<Characteristic>(new CharacteristicConfig());
            modelBuilder.AddConfiguration<CharacteristicFeature>(new CharacteristicObjectConfig());
            modelBuilder.AddConfiguration<UserClaim>(new ClaimConfig());


            modelBuilder.AddConfiguration<DatasetTemplate>(new DomainTemplateConfig());
            modelBuilder.AddConfiguration<DatasetTemplateField>(new DomainTemplateVariableConfig());
            modelBuilder.AddConfiguration<TemplateFieldDB>(new TemplateFieldDBsConfig());
            modelBuilder.AddConfiguration<CVterm>(new CVtermConfig());
            modelBuilder.AddConfiguration<DB>(new DbConfig());
            modelBuilder.AddConfiguration<Dbxref>(new DBxrefConfig());
            modelBuilder.AddConfiguration<Dictionary>(new DictionaryConfig());

            
            modelBuilder.AddConfiguration<DataFile>(new DatafileConfig());
            modelBuilder.AddConfiguration<Dataset>(new DatasetConfig());
            modelBuilder.AddConfiguration<DatasetDatafile>(new DatasetDatafileConfig());

            modelBuilder.AddConfiguration<PlatformTM.Core.Domain.Model.Observation>(new ObservationConfig());
            modelBuilder.AddConfiguration<ObservationQualifier>(new ObservationQualifiersConfig());
            modelBuilder.AddConfiguration<ObservationSynonym>(new ObservationSynonymConfig());
            modelBuilder.AddConfiguration<ObservationTiming>(new ObservationTimingsConfig());

            modelBuilder.AddConfiguration<Project>(new ProjectConfig());
            modelBuilder.AddConfiguration<ProjectUser>(new ProjectUserConfig());

            modelBuilder.AddConfiguration<Study>(new StudyConfig());
            modelBuilder.AddConfiguration<StudyDataset>(new StudyDatasetConfig());
            modelBuilder.AddConfiguration<StudyArm>(new StudyArmConfig());

            modelBuilder.AddConfiguration<HumanSubject>(new SubjectConfig());

            modelBuilder.AddConfiguration<TimePoint>(new TimePointConfig());

            modelBuilder.AddConfiguration<User>(new UserConfig());


            modelBuilder.AddConfiguration<VariableDefinition>(new VariableDefConfig());
            modelBuilder.AddConfiguration<VariableReference>(new VariableRefConfig());

            modelBuilder.AddConfiguration<Visit>(new VisitConfig());

        }

        public Task<int> SaveChangesAsync()
        {
            try
            {
                return base.SaveChangesAsync();
            }
            catch (Exception e)
            {
                while (e.InnerException != null)
                    e = e.InnerException;
                throw e;
            }
        }

        public void InitDB()
        {
            WaitForDBInit();
            Database.Migrate();
        }

        private void WaitForDBInit()
        {
            int retries = 1;
            while (retries < 7)
            {
                try
                {
                    Console.WriteLine("Connecting to db. Trial: {0}", retries);
                    Database.OpenConnection();
                    Database.CloseConnection();
                    break;
                }
                catch (MySqlException)
                {
                    Thread.Sleep((int)Math.Pow(2, retries) * 1000);
                    retries++;
                }
            }
        }
    }
}

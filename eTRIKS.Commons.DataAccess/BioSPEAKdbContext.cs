using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.Core.Domain.Model.Base;
using eTRIKS.Commons.Core.Application.AccountManagement;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Core.Domain.Model.Users.Datasets;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Model.ControlledTerminology;
using eTRIKS.Commons.Core.Domain.Model.DatasetModel;
using eTRIKS.Commons.Core.Domain.Model.DesignElements;
using eTRIKS.Commons.Core.Domain.Model.Templates;
using eTRIKS.Commons.Core.Domain.Model.Timing;
using eTRIKS.Commons.Core.Domain.Model.Users;
using eTRIKS.Commons.Core.Domain.Model.Users.Queries;
using eTRIKS.Commons.Core.JoinEntities;
using eTRIKS.Commons.DataAccess.Configuration;
using eTRIKS.Commons.DataAccess.EntityConfigurations;
using eTRIKS.Commons.DataAccess.Extensions;
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
                Debug.WriteLine("Retrieving ",typeof(TEntity).Name);
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
            if (typeof(TEntity) == typeof(UserDataset) || typeof(TEntity) == typeof(CombinedQuery))
            {
                var mongoClient = new MongoClient(_dbsettings.Value.MongoDBconnection);
                var mongodb = mongoClient.GetDatabase(_dbsettings.Value.noSQLDatabaseName);
                var MongoRepository = new GenericMongoRepository<TEntity, TPrimaryKey>(mongodb,"userDatasets");
                _repositories.Add(typeof(TEntity), MongoRepository);
                return MongoRepository;
            }
            if (typeof(TEntity) == typeof(Core.Domain.Model.ObservationModel.Observation) )
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

        public void Register<TEntity, TPrimaryKey>(IRepository<TEntity, TPrimaryKey> repository) where TEntity : Identifiable<TPrimaryKey>, IEntity<TPrimaryKey>
        {
            _repositories.Add(typeof(TEntity), repository);
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
            modelBuilder.AddConfiguration<Activity>(new ActivityConfig());
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

            modelBuilder.AddConfiguration<Observation>(new ObservationConfig());
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
                //tran.Dispose();
                while (e.InnerException != null)
                    e = e.InnerException;
                throw e;
            }
        }
    }
}

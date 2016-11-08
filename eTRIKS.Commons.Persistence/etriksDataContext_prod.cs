using System;
using System.Collections.Generic;
using System.Linq;
using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.Core.Domain.Model.Base;
using eTRIKS.Commons.DataAccess;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using eTRIKS.Commons.Core.Domain.Model.Data.SDTM;
using eTRIKS.Commons.Core.Application.AccountManagement;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Core.Domain.Model.Users.Datasets;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace eTRIKS.Commons.Persistence
{
    public class EtriksDataContextProd : DbContext, IServiceUoW
    {
        //private readonly IDataContext _dataContext;

        private readonly Dictionary<Type, object> _repositories;
        private IUserRepository userRepository;
        private IUserAccountRepository _userAccountRepository;
        private bool _disposed;
        

        public EtriksDataContextProd(DbContextOptions<EtriksDataContextProd> options) : base(options)
        {
            //"name=eTRIKScontext_MySQL"
            //_dataContext = context;
            
            _repositories = new Dictionary<Type, object>();
            _disposed = false;
        }

        public void setSDTMentityDescriptor(SdtmRowDescriptor descriptor)
        {
            SdtmSerializer.sdtmEntityDescriptor = descriptor;
        }

        public void AddClassMap(string fieldname, string propertyName)
        {
            BsonSerializationInfo info = new BsonSerializationInfo(fieldname, new StringSerializer(), typeof(string));
            SubjectObsSerializer.DynamicMappers.Remove(propertyName);
            SubjectObsSerializer.DynamicMappers.Add(propertyName, info);
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

        //public Task<int> SaveChangesAsync()
        //{

        //}

        public new void Dispose()
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

            /*
            modelBuilder.Entity<DomainTemplateMap>();
            modelBuilder.Entity<DomainVariableTemplateMap>();
            modelBuilder.Entity<DatasetMap>();
            modelBuilder.Entity<ActivityMap>();
            modelBuilder.Entity<CVtermMap>();
            modelBuilder.Entity<DbMap>();
            modelBuilder.Entity<DBxrefMap>();
            modelBuilder.Entity<DerivedVariablePropertiesMap>();
            modelBuilder.Entity<DictionaryMap>();
            modelBuilder.Entity<StudyMap>();
            modelBuilder.Entity<VariableDefMap>();
            modelBuilder.Entity<VariableRefMap>();
            modelBuilder.Entity<ObservationMap>();
            modelBuilder.Entity<ProjectMap>();
            modelBuilder.Entity<DataFileMap>();
            modelBuilder.Entity<VisitMap>();
            modelBuilder.Entity<SubjectMap>();
            modelBuilder.Entity<BioSampleMap>();
            modelBuilder.Entity<CharacterisitcMap>();
            modelBuilder.Entity<CharacteristicObjectMap>();
            modelBuilder.Entity<ArmMap>();
            modelBuilder.Entity<UserMap>();
            modelBuilder.Entity<AccountMap>();
            modelBuilder.Entity<ClaimMap>();
            */
           
        }

        public Task<int> SaveChangesAsync()
        {
            throw new NotImplementedException();
        }
    }
}

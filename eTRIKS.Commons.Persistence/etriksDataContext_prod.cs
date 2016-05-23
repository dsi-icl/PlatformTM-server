using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.Core.Domain.Model.Base;
using eTRIKS.Commons.DataAccess;
using eTRIKS.Commons.Persistence.Mapping;
using System.Transactions;
using Microsoft.AspNet.Identity;
using MongoDB.Bson.Serialization;
using eTRIKS.Commons.Core.Domain.Model;
using MongoDB.Bson.Serialization.Serializers;
using Microsoft.AspNet.Identity.EntityFramework;
using eTRIKS.Commons.DataAccess.UserManagement;

namespace eTRIKS.Commons.Persistence {

    [DbConfigurationType(typeof(MySql.Data.Entity.MySqlEFConfiguration))]
    public class EtriksDataContextProd : IdentityDbContext<ApplicationUser>, IServiceUoW
    {
        //private readonly IDataContext _dataContext;

        private readonly Dictionary<Type, object> _repositories;
        private IUserRepository<ApplicationUser,IdentityResult> userAuthRepository;
        private bool _disposed;

        public EtriksDataContextProd() : base("name=eTRIKScontext_MySQL")
        {
            //_dataContext = context;
            Configuration.ProxyCreationEnabled = false;
            Database.SetInitializer<EtriksDataContextProd>(null);

            DbConfiguration.SetConfiguration(new MySql.Data.Entity.MySqlEFConfiguration());

            //BsonSerializer.RegisterSerializer(typeof(SubjectObservation), new SubjectObsSerializer());
            //BsonSerializer.RegisterSerializer(typeof(SdtmEntity), new SdtmSerializer());
            //BsonSerializer.RegisterSerializer(typeof(MongoDocument),new MongoDocumentSerializer());

            

            

      
            _repositories = new Dictionary<Type, object>();
            //userAuthRepository = new UserAuthRepository(this);
            //_repositories.Add(typeof(ApplicationUser), userAuthRepository);

            this.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
            _disposed = false;
        }

        public void setSDTMentityDescriptor(SdtmEntityDescriptor descriptor)
        {
            SdtmSerializer.sdtmEntityDescriptor = descriptor;
        }

        public void AddClassMap(string fieldname, string propertyName)
        {
            BsonSerializationInfo info = new BsonSerializationInfo(fieldname, new StringSerializer(), typeof(string));
            SubjectObsSerializer.DynamicMappers.Remove(propertyName);
            SubjectObsSerializer.DynamicMappers.Add(propertyName, info);
        }

        public IUserRepository<TEntity,TResult> GetUserRepository<TEntity,TResult>()
        {
            if (userAuthRepository == null)
            {
                userAuthRepository = new UserAuthRepository(this);
            }
            return userAuthRepository as IUserRepository<TEntity, TResult>;
        }

        public IRepository<TEntity, TPrimaryKey> GetRepository<TEntity, TPrimaryKey>() 
            where TEntity : Identifiable<TPrimaryKey>, IEntity<TPrimaryKey> {

            // Checks if the Dictionary Key contains the Model class
            if (_repositories.Keys.Contains(typeof(TEntity))){
                // Return the repository for that Model class
                return _repositories[typeof(TEntity)] as IRepository<TEntity,TPrimaryKey>;
            }

            // If the repository for that Model class doesn't exist, create it
            if (typeof(TEntity).Name.Equals("SubjectObservation"))
            {
                var MongoRepository = new GenericMongoRepository<TEntity, TPrimaryKey>();
                _repositories.Add(typeof(TEntity), MongoRepository);
                return MongoRepository;
            }

            if (typeof(TEntity).Name.Equals("MongoDocument"))
            {
                var MongoRepository = new GenericMongoRepository<TEntity, TPrimaryKey>();
                _repositories.Add(typeof(TEntity), MongoRepository);
                return MongoRepository;
            }

            //if (typeof(TEntity).Name.Equals("Subject"))
            //{
            //    var MongoRepository = new GenericMongoRepository<TEntity, TPrimaryKey>();
            //    _repositories.Add(typeof(TEntity), MongoRepository);
            //    return MongoRepository;
            //}
            if (typeof(TEntity).Name.Equals("SdtmEntity"))
            {
                var MongoRepository = new GenericMongoRepository<TEntity, TPrimaryKey>();
                _repositories.Add(typeof(TEntity), MongoRepository);
                return MongoRepository;
            }
            

            //var repository = new GenericRepository<TEntity, TPrimaryKey>(base.Set<TEntity>());
            var repository = new GenericRepository<TEntity, TPrimaryKey>(this);

            // Add it to the dictionary
            _repositories.Add(typeof(TEntity), repository);

            return repository;
        }

        //public IRepository<TEntity, TPrimaryKey> GetDatasetRepository<TEntity, TPrimaryKey>()
        //    where TEntity : Identifiable<TPrimaryKey>, IEntity<TPrimaryKey>
        //{
        //    if (typeof(TEntity).Name.Equals("SubjectObservation"))
        //    {
        //        var MongoRepository = new GenericMongoRepository<TEntity, TPrimaryKey>();
        //        _repositories.Add(typeof(TEntity), MongoRepository);
        //        return MongoRepository;
        //    }

        //    if (typeof(TEntity).Name.Equals("MongoDocument"))
        //    {
        //        var MongoRepository = new GenericMongoRepository<TEntity, TPrimaryKey>();
        //        _repositories.Add(typeof(TEntity), MongoRepository);
        //        return MongoRepository;
        //    }

        //    if (typeof(TEntity).Name.Equals("Subject"))
        //    {
        //        var MongoRepository = new GenericMongoRepository<TEntity, TPrimaryKey>();
        //        _repositories.Add(typeof(TEntity), MongoRepository);
        //        return MongoRepository;
        //    }
        //    if (typeof(TEntity).Name.Equals("Biospecimen"))
        //    {
        //        var MongoRepository = new GenericMongoRepository<TEntity, TPrimaryKey>();
        //        _repositories.Add(typeof(TEntity), MongoRepository);
        //        return MongoRepository;
        //    }
        //}

        public string Save() {

            //using (var tran = new TransactionScope())
            //{
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
            //}
        }


        public void Dispose(){
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
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<Microsoft.AspNet.Identity.EntityFramework.IdentityRole>()
            //.Property(c => c.Name).HasMaxLength(128).IsRequired();

            //modelBuilder.Entity<Microsoft.AspNet.Identity.EntityFramework.IdentityUser>().ToTable("AspNetUsers")
            //    .Property(c => c.UserName).HasMaxLength(128).IsRequired();

            modelBuilder.Configurations.Add(new DomainTemplateMap());
            modelBuilder.Configurations.Add(new DomainVariableTemplateMap());
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
            modelBuilder.Configurations.Add(new ObservationMap());
            modelBuilder.Configurations.Add(new ProjectMap());
            modelBuilder.Configurations.Add(new DataFileMap());
            modelBuilder.Configurations.Add(new VisitMap());
            modelBuilder.Configurations.Add(new SubjectMap());
            modelBuilder.Configurations.Add(new BioSampleMap());
            modelBuilder.Configurations.Add(new CharacterisitcMap());
            modelBuilder.Configurations.Add(new CharacteristicObjectMap());
        }



    }
}

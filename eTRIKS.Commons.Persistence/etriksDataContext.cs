
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Core.Domain.Model.ControlledTerminology;
using eTRIKS.Commons.Core.Domain.Model.Templates;
using eTRIKS.Commons.DataAccess;
using eTRIKS.Commons.Persistence.Mapping;

namespace eTRIKS.Commons.Persistence
{
    //CURRENTLY NOT IN USE
    public class eTRIKSDataContext : DbContext, IDataContext
    {
        public eTRIKSDataContext() : base("name=eTRIKScontext_MySQL")
        {
            Database.SetInitializer<eTRIKSDataContext>(null);
            
        }


        //TODO: not sure if these DbSet declarations still need to be here.
        public DbSet<DomainTemplate> DomainTemplates { get; set; }
        public DbSet<DomainTemplateVariable> DomainTemplateVariables { get; set; }

        public DbSet<Study> Studies { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<Dataset> Datasets { get; set; }
        public DbSet<VariableDefinition> Variables { get; set; }
        public DbSet<DerivedMethod> DerivedVariableProps { get; set; }
        public DbSet<VariableReference> DatasetReferencedVariables { get; set; }


        public DbSet<CVterm> CVterms { get; set; }
        public DbSet<DB> DBs { get; set; }
        public DbSet<DBxref> DBxrefs { get; set; }
        public DbSet<Dictionary> Dictionaries { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new DomainDatasetMap());
            modelBuilder.Configurations.Add(new DomainVariableMap());
            modelBuilder.Configurations.Add(new DatasetMap());
            modelBuilder.Configurations.Add(new ActivityMap());
            modelBuilder.Configurations.Add(new CVtermMap());
            modelBuilder.Configurations.Add(new DbMap());
            modelBuilder.Configurations.Add(new DBxrefMap());
           // modelBuilder.Configurations.Add(new DerivedVariablePropertiesMap());
            modelBuilder.Configurations.Add(new DictionaryMap());
            modelBuilder.Configurations.Add(new StudyMap());
            modelBuilder.Configurations.Add(new VariableDefMap());
            modelBuilder.Configurations.Add(new VariableRefMap());
        }

        public IDbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            return base.Set<TEntity>();
        }
    }
}

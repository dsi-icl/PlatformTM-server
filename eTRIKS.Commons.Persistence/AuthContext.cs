using eTRIKS.Commons.DataAccess.UserManagement;
using Microsoft.AspNet.Identity.EntityFramework;
using MySql.Data.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTRIKS.Commons.Persistence
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class AuthContext : IdentityDbContext<ApplicationUser>
    {
        public AuthContext() : base("AuthContext"){
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Fix asp.net identity 2.0 tables under MySQL
            // Explanations: primary keys can easily get too long for MySQL's
            // (InnoDB's) stupid 767 bytes limit.
            // With the two following lines we rewrite the generation to keep
            // those columns "short" enough
            modelBuilder.Entity<IdentityRole>()
            .Property(c => c.Name)
            .HasMaxLength(128)
            .IsRequired();

            // We have to declare the table name here, otherwise IdentityUser
            // will be created
            modelBuilder.Entity<ApplicationUser>()
            .ToTable("AspNetUsers")
            .Property(c => c.UserName)
            .HasMaxLength(128)
            .IsRequired();
            #endregion
        }
    }
 

}

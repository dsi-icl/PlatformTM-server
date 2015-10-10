using System.Data.Entity;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.Persistence;
using eTRIKS.Commons.DataParser.IOFileManagement;
using eTRIKS.Commons.DataAccess.MongoDB;
using Microsoft.AspNet.Identity.EntityFramework;
using eTRIKS.Commons.DataAccess.UserManagement;
using eTRIKS.Commons.DataAccess;

namespace eTRIKS.Commons.WebAPI.DependencyResolution.Installers
{
    public class RepositoriesInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {

            container.Register(

                //TODO: remove EF and Persistence dependencies
                
                Component.For<IdentityDbContext<ApplicationUser>, IServiceUoW>()
                    .ImplementedBy<etriksDataContext_prod>(),

                Component.For<FileHandler>(),

                Component.For<MongoDbDataRepository>(),

                Classes
                    .FromAssemblyNamed("eTRIKS.Commons.DataAccess")
                    .BasedOn(typeof(IRepository<,>))
                    .WithServiceDefaultInterfaces(),

                    Component.For<IUserRepository<ApplicationUser>>().ImplementedBy<UserAuthRepository<ApplicationUser>>(),

                Classes
                     .FromAssemblyNamed("eTRIKS.Commons.Service")
                     .InNamespace("eTRIKS.Commons.Service.Services")
                     .WithServiceSelf()
                     .WithServiceDefaultInterfaces(),

                Classes
                     .FromAssemblyNamed("eTRIKS.Commons.Service")
                     .InNamespace("eTRIKS.Commons.Service.Services.UserServices")
                     .WithServiceSelf()
                     .WithServiceDefaultInterfaces());


            //container.Register(
            //    );

            //container.Register(
            //     );
             

           
            

        }
    }
}
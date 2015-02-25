using System.Data.Entity;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.Persistence;
using eTRIKS.Commons.DataParser.IOFileManagement;
using eTRIKS.Commons.DataParser.MongoDBAccess;

namespace eTRIKS.Commons.WebAPI.DependencyResolution.Installers
{
    public class RepositoriesInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {

            container.Register(

                //TODO: remove EF and Persistence dependencies
                Component.For<DbContext, IServiceUoW>()
                    .ImplementedBy<etriksDataContext_dev>(),

                Component.For<FileHandler>(),

                Component.For<MongoDbDataServices>(),

                Classes
                    .FromAssemblyNamed("eTRIKS.Commons.DataAccess")
                    .BasedOn(typeof(IRepository<,>))
                    .WithServiceDefaultInterfaces(),

                Classes
                     .FromAssemblyNamed("eTRIKS.Commons.Service")
                     .InNamespace("eTRIKS.Commons.Service.Services")
                     .WithServiceSelf()
                     .WithServiceDefaultInterfaces());


            //container.Register(
            //    );

            //container.Register(
            //     );
             

           
            

        }
    }
}
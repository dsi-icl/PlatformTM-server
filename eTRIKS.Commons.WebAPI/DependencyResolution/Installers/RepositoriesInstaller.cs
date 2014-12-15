using System.Data.Entity;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.Persistence;

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


                    //Classes
                    //.FromAssemblyNamed("eTRIKS.Commons.Persistence")
                    //.InNamespace("eTRIKS.Commons.Persistence")
                    //.WithServiceFromInterface(),


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
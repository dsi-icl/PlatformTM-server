using System.Web.Http;
using System.Web.Mvc;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace eTRIKS.Commons.WebAPI.DependencyResolution.Installers
{
    public class ControllersInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Classes.
                    FromThisAssembly().
                    BasedOn<IController>().
                    If(c => c.Name.EndsWith("Controller")).
                    LifestyleTransient(),
                    
                    
                Classes.
                    FromThisAssembly().
                    BasedOn<ApiController>().
                    If(c => c.Name.EndsWith("Controller")).
                    LifestyleTransient()
                    );

           
        }
    }
}
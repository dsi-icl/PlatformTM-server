using System;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Mvc;
using Castle.Windsor;
using Castle.Windsor.Installer;
using eTRIKS.Commons.WebAPI.DependencyResolution.Plumbing;

namespace eTRIKS.Commons.WebAPI.DependencyResolution
{
    public class ContainerBootstrapper : IContainerAccessor, IDisposable
    {
        readonly IWindsorContainer _container;

        ContainerBootstrapper(IWindsorContainer container)
        {
            this._container = container;
        }

        public IWindsorContainer Container
        {
            get { return _container; }
        }

        public static ContainerBootstrapper Bootstrap()
        {
            
            
            var container = new WindsorContainer()
                .Install(FromAssembly.This());

            ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(container));

            GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpControllerActivator),
                new WindsorControllerFactory(container));


            return new ContainerBootstrapper(container);

            
        }

        public void Dispose()
        {
            Container.Dispose();
        }
    }
}
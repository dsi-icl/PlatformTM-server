using System;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Mvc;
using Castle.Windsor;
using Castle.Windsor.Installer;
using eTRIKS.Commons.WebAPI.DependencyResolution.Plumbing;
using System.Collections.Generic;
using System.Web.Http.Dependencies;

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

        public static ContainerBootstrapper Bootstrap(HttpConfiguration HttpConfiguration)
        {
            
            
            var container = new WindsorContainer()
                .Install(FromAssembly.This());

            ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(container));

            //var httpDependencyResolver = new WindsorHttpDependencyResolver(container);

           // GlobalConfiguration.Configuration.Services.Replace.ServiceResolver.SetResolver(
            //        serviceType => container.Resolve(serviceType),
            //        serviceType => container.ResolveAll(serviceType).Cast<object>());

            HttpConfiguration.Services.Replace(typeof(IHttpControllerActivator),
                new WindsorControllerFactory(container));


            return new ContainerBootstrapper(container);

            
        }

        public void Dispose()
        {
            Container.Dispose();
        }
    }

    //internal sealed class WindsorHttpDependencyResolver : IDependencyResolver
    //{
    //    private readonly IWindsorContainer _container;

    //    public WindsorHttpDependencyResolver(IWindsorContainer container)
    //    {
    //        if (container == null)
    //        {
    //            throw new ArgumentNullException("container");
    //        }

    //        _container = container;
    //    }

    //    public object GetService(Type t)
    //    {
    //        return _container.Kernel.HasComponent(t)
    //         ? _container.Resolve(t) : null;
    //    }

    //    public IEnumerable<object> GetServices(Type t)
    //    {
    //        return _container.ResolveAll(t)
    //        .Cast<object>().ToArray();
    //    }

    //    public IDependencyScope BeginScope()
    //    {
    //        return new WindsorDependencyScope(_container);
    //    }

    //    public void Dispose()
    //    {
    //    }
    //}
}
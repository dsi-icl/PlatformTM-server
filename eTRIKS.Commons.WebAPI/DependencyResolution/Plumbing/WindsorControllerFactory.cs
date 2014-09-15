using System;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.Windsor;

namespace eTRIKS.Commons.WebAPI.DependencyResolution.Plumbing
{
    public class WindsorControllerFactory : DefaultControllerFactory, IHttpControllerActivator
    {
        readonly IWindsorContainer _container;

        public WindsorControllerFactory(IWindsorContainer container)
        {
            this._container = container;
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {            
			if (controllerType != null && _container.Kernel.HasComponent(controllerType))
				return (IController)_container.Resolve(controllerType);

			return base.GetControllerInstance(requestContext, controllerType);
        }

        public override void ReleaseController(IController controller)
        {
            _container.Release(controller);
        }

        public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
        {
            var APIcontroller =
            (IHttpController)this._container.Resolve(controllerType);

            request.RegisterForDispose(
                new ReleaseApiController(
                    () => this._container.Release(APIcontroller)));

            return APIcontroller;
        }

        private class ReleaseApiController : IDisposable
        {
            private readonly Action _release;

            public ReleaseApiController(Action release)
            {
                _release = release;
            }

            public void Dispose()
            {
                _release();
            }
        }

    }
}
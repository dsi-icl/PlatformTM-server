using System;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Mvc;
using Castle.Windsor;
using Castle.Windsor.Installer;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Persistence;
using eTRIKS.Commons.WebAPI.DependencyResolution.Plumbing;
using System.Collections.Generic;
using System.Web.Http.Dependencies;
using eTRIKS.Commons.Core.Domain.Model.DatasetModel.SDTM;
using MongoDB.Bson.Serialization;

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
            var container = new WindsorContainer().Install(FromAssembly.This());

            ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(container));

            HttpConfiguration.Services.Replace(typeof(IHttpControllerActivator),
                new WindsorControllerFactory(container));

            //BsonSerializer.RegisterSerializer(typeof(SubjectObservation), new SubjectObsSerializer());
            //BsonSerializer.RegisterSerializer(typeof(SdtmRow), new SdtmSerializer());
            //BsonSerializer.RegisterSerializer(typeof(MongoDocument), new MongoDocumentSerializer());


            return new ContainerBootstrapper(container);
        }

        public void Dispose()
        {
            Container.Dispose();
        }
    }
}
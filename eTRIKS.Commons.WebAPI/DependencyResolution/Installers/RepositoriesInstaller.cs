using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.DataAccess.MongoDB;
using eTRIKS.Commons.DataAccess;
using System;
using eTRIKS.Commons.Service.Services.UserManagement;
using eTRIKS.Commons.Persistence;

namespace eTRIKS.Commons.WebAPI.DependencyResolution.Installers
{
    public class RepositoriesInstaller : IWindsorInstaller
	{
		public void Install(IWindsorContainer container, IConfigurationStore store)
		{

			container.Register(

			   //TODO: remove EF and Persistence dependencies

			    Component.For<DbContext, IServiceUoW>().LifestylePerWebRequest()
                    .ImplementedBy<EtriksDataContextProd>(),

			    Component.For<IUserRepository>()
                    .ImplementedBy<UserRepository>(),

			    Component.For<IUserStore<UserAccount, Guid>>()
                    .ImplementedBy<UserStore>(),


			    Component.For<UserAccountService>()
                    .LifestylePerWebRequest(),

			    Component.For<MongoDbDataRepository>(),

				Classes
					.FromAssemblyNamed("eTRIKS.Commons.DataAccess")
					.BasedOn(typeof(IRepository<,>)).LifestylePerWebRequest()
					.WithServiceDefaultInterfaces(),

				Classes
					 .FromAssemblyNamed("eTRIKS.Commons.Service")
					 .InNamespace("eTRIKS.Commons.Service.Services").LifestylePerWebRequest()
					 .WithServiceSelf()
					 .WithServiceDefaultInterfaces(),


				Classes
					 .FromAssemblyNamed("eTRIKS.Commons.Service")
					 .InNamespace("eTRIKS.Commons.Service.Services.UserServices")
					 .WithServiceSelf()
					 .WithServiceDefaultInterfaces());

		}
	}
}
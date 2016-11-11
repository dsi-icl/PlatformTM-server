using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.DataAccess;
using Microsoft.AspNet.Identity;
using System;
using eTRIKS.Commons.Service.Services.UserManagement;
using eTRIKS.Commons.Persistence;
using System.Data.Entity;

namespace eTRIKS.Commons.WebAPI.DependencyResolution.Installers
{
	public class RepositoriesInstaller : IWindsorInstaller
	{
		public void Install(IWindsorContainer container, IConfigurationStore store)
		{

			container.Register(

			   //TODO: remove EF and Persistence dependencies

			    Component.For<DbContext, IServiceUoW>().LifestylePerWebRequest()
                    .ImplementedBy<BioSPEAKdbContext>(),

			    Component.For<IUserRepository>()
                    .ImplementedBy<UserRepository>(),

			    Component.For<IUserStore<UserAccount, Guid>>()
                    .ImplementedBy<UserStore>(),


			    Component.For<UserAccountService>()
                    .LifestylePerWebRequest(),

			   

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
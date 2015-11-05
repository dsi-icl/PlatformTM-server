using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.DataAccess.UserManagement;
using eTRIKS.Commons.Service.Services.UserServices;
using eTRIKS.Commons.WebAPI.DependencyResolution;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
//using System.Web.Mvc;

[assembly: OwinStartup(typeof(eTRIKS.Commons.WebAPI.Startup))]
//[assembly: ApplicationShutdownMethod(typeof(WindsorActivator), "Shutdown")]
namespace eTRIKS.Commons.WebAPI
{
    public class Startup
    {
        static ContainerBootstrapper _bootstrapper;
        public static HttpConfiguration HttpConfiguration { get; private set; }
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration HttpConfiguration = new HttpConfiguration();
            
            //GlobalConfiguration.Configure(WebApiConfig.Register);
            //
            //
           // 
            
            //Bootstrap & Configure Windsor Container
            _bootstrapper = ContainerBootstrapper.Bootstrap(HttpConfiguration);
          
            //NEED TO CHECK HOW TO DISPOSE CONTAINER


            //GlobalConfiguration.Configure(WebApiConfig.Register);

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //var container = new WindsorContainer().Install(
            //new ControllerInstaller(),
            //new DefaultInstaller());
            //var httpDependencyResolver = new WindsorHttpDependencyResolver(container);

            //AreaRegistration.RegisterAllAreas();
            //GlobalConfiguration.Configure(WebApiConfig.Register);
            //FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            //RouteConfig.RegisterRoutes(RouteTable.Routes);
            //BundleConfig.RegisterBundles(BundleTable.Bundles);

           // HttpConfiguration config = new HttpConfiguration();
           // var httpDependencyResolver = new WindsorHttpDependencyResolver(container);
           // config.DependencyResolver = httpDependencyResolver;
 
            ConfigureOAuth(app,HttpConfiguration);
 
            WebApiConfig.Register(HttpConfiguration);


            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            app.UseWebApi(HttpConfiguration);
 
        }
 
        public void ConfigureOAuth(IAppBuilder app, HttpConfiguration config)
        {
            //Are these needed given IoC
            //app.CreatePerOwinContext(ApplicationDbContext.Create);
            //app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);

            //var repo = _bootstrapper.Container.Resolve<IUserRepository<ApplicationUser>>();
            OAuthBearerAuthenticationOptions OAuthBearerOptions = new OAuthBearerAuthenticationOptions();
 
            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                //For Dev enviroment only (on production should be AllowInsecureHttp = false)
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
                Provider = new SimpleAuthorizationServerProvider(_bootstrapper.Container.Resolve<IUserRepository<ApplicationUser>>())
            };
 
            // Token Generation
            app.UseOAuthAuthorizationServer(OAuthServerOptions);
 
            //Token Consumption
            app.UseOAuthBearerAuthentication(OAuthBearerOptions);
 
        }
    }

}
using eTRIKS.Commons.DataAccess.Configuration;
using eTRIKS.Commons.Service.Services.Authentication;
using eTRIKS.Commons.Service.Services.UserManagement;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Diagnostics;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace eTRIKS.Commons.WebAPI
{
    public class Startup
    {

        //static ContainerBootstrapper _bootstrapper;
        //public static HttpConfiguration HttpConfiguration { get; private set; }
        //public void Configuration(IAppBuilder app)

        const string TokenAudience = "ExampleAudience";
        const string TokenIssuer = "ExampleIssuer";
        private RsaSecurityKey key;
        private TokenAuthOptions tokenOptions;
        //private readonly UserAccountService _accountService;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsEnvironment("Development"))
            //if (env.IsDevelopment()) 
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);

            ////////services.AddMvc();

            // Added - uses IOptions<T> for your settings.
            services.AddOptions();

            // Added - Confirms that we have a home for our DataAccessSettings
            services.Configure<DataAccessSettings>(Configuration.GetSection("DataAccessSettings"));


            ////////////            HttpConfiguration HttpConfiguration = new HttpConfiguration();

            ////////////            //Bootstrap & Configure Windsor Container
            ////////////            _bootstrapper = ContainerBootstrapper.Bootstrap(HttpConfiguration);

            ////////////            //NEED TO CHECK HOW TO DISPOSE CONTAINER

            ////////////            AreaRegistration.RegisterAllAreas();
            ////////////            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            ////////////            RouteConfig.RegisterRoutes(RouteTable.Routes);
            ////////////            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ////////////            ConfigureOAuth(app);

            ////////////            WebApiConfig.Register(HttpConfiguration);

            ////////////            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            ////////////            app.UseWebApi(HttpConfiguration);


            ////////////        }


            ////////        public void ConfigureOAuth(IAppBuilder app)
            ////////        {
            ////////            //Are these needed given IoC
            ////////            //app.CreatePerOwinContext(ApplicationDbContext.Create);
            ////////            //app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);

            ////////            //var repo = _bootstrapper.Container.Resolve<IUserRepository<ApplicationUser>>();
            ////////            OAuthBearerAuthenticationOptions OAuthBearerOptions = new OAuthBearerAuthenticationOptions();

            ////////            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            ////////            {
            ////////                //TODO: For Dev enviroment only (on production should be AllowInsecureHttp = false)
            ////////                AllowInsecureHttp = true,
            ////////                TokenEndpointPath = new PathString("/token"),
            ////////                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
            ////////                Provider = new CustomOAuthServerProvider(_bootstrapper.Container.Resolve<UserAccountService>())
            ////////            };

            ////////            // Token Generation
            ////////            app.UseOAuthAuthorizationServer(OAuthServerOptions);

            ////////            //Token Consumption
            ////////            app.UseOAuthBearerAuthentication(OAuthBearerOptions);










            // *** CHANGE THIS FOR PRODUCTION USE ***
            // Here, we're generating a random key to sign tokens - obviously this means
            // that each time the app is started the key will change, and multiple servers 
            // all have different keys. This should be changed to load a key from a file 
            // securely delivered to your application, controlled by configuration.
            //
            // See the RSAKeyUtils.GetKeyParameters method for an examle of loading from
            // a JSON file.
            RSAParameters keyParams = RSAKeyUtils.GetRandomKey();

            // Create the key, and a set of token options to record signing credentials 
            // using that key, along with the other parameters we will need in the 
            // token controlller.
            key = new RsaSecurityKey(keyParams);
            tokenOptions = new TokenAuthOptions()
            {
                Audience = TokenAudience,
                Issuer = TokenIssuer,
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256Signature)
            };

            // Save the token options into an instance so they're accessible to the 
            // controller.
            //replaced AddInstance with AddSingleto
            services.AddSingleton<TokenAuthOptions>(tokenOptions);
            //services.AddInstance<TokenAuthOptions>(tokenOptions);

            // Enable the use of an [Authorize("Bearer")] attribute on methods and classes to protect.
            services.AddAuthorization(auth =>
            {
                auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme‌​)
                    .RequireAuthenticatedUser().Build());
            });

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseApplicationInsightsRequestTelemetry();

            app.UseApplicationInsightsExceptionTelemetry();








            app.UseIISPlatformHandler();
            // Register a simple error handler to catch token expiries and change them to a 401, 
            // and return all other errors as a 500. This should almost certainly be improved for
            // a real application.
            app.UseExceptionHandler(appBuilder =>
            {
                appBuilder.Use(async (context, next) =>
                {
                    var error = context.Features[typeof(IExceptionHandlerFeature)] as IExceptionHandlerFeature;
                    // This should be much more intelligent - at the moment only expired 
                    // security tokens are caught - might be worth checking other possible 
                    // exceptions such as an invalid signature.
                    if (error != null && error.Error is SecurityTokenExpiredException)
                    {
                        context.Response.StatusCode = 401;
                        // What you choose to return here is up to you, in this case a simple 
                        // bit of JSON to say you're no longer authenticated.
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(
                            JsonConvert.SerializeObject(
                                new { authenticated = false, tokenExpired = true }));
                    }
                    else if (error != null && error.Error != null)
                    {
                        context.Response.StatusCode = 500;
                        context.Response.ContentType = "application/json";
                        // TODO: Shouldn't pass the exception message straight out, change this.
                        await context.Response.WriteAsync(
                            JsonConvert.SerializeObject
                            (new { success = false, error = error.Error.Message }));
                    }
                    // We're not trying to handle anything else so just let the default 
                    // handler handle.
                    else await next();
                });
            });

            var options = new JwtBearerOptions();
            options.TokenValidationParameters.IssuerSigningKey = key;
            options.TokenValidationParameters.ValidAudience = tokenOptions.Audience;
            options.TokenValidationParameters.ValidIssuer = tokenOptions.Issuer;
            options.TokenValidationParameters.ValidateIssuerSigningKey = true;
            options.TokenValidationParameters.ValidateLifetime = true;
            options.TokenValidationParameters.ClockSkew = TimeSpan.FromMinutes(0);

            app.UseJwtBearerAuthentication(options);

            // Configure the HTTP request pipeline.
            app.UseStaticFiles();

            // Add MVC to the request pipeline.
            app.UseMvc();
        }
        /// public static void Main(string[] args) => WebApplication.Run(args);
    }

}
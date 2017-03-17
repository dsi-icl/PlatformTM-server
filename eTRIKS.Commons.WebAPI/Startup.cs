using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using eTRIKS.Commons.DataAccess.Configuration;
using eTRIKS.Commons.Service.Services;
using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.DataAccess;
using MySQL.Data.Entity.Extensions;
using eTRIKS.Commons.Service.Services.UserManagement;
using eTRIKS.Commons.Core.Application.AccountManagement;
using eTRIKS.Commons.Core.Domain.Model.ObservationModel;
using eTRIKS.Commons.Service.Configuration;
using eTRIKS.Commons.Service.Services.Loading.HdDataLoader;
using eTRIKS.Commons.Service.Services.Loading.SDTM;
using eTRIKS.Commons.WebAPI.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace eTRIKS.Commons.WebAPI
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsEnvironment("Development"))
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
               

            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });
            services.AddOptions();
            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddAuthorization(options =>
            {
                options.AddPolicy(
                    "CanAccessAdminArea",
                    policyBuilder => policyBuilder.RequireRole("ADMIN", "OWNER"));
                options.AddPolicy(
                   "CanImportData",
                   policyBuilder => policyBuilder.RequireClaim("CanImportData"));
            });

            services.Configure<DataAccessSettings>(Configuration.GetSection("DBSettings"));
            services.Configure<FileStorageSettings>(Configuration.GetSection("FileStorageSettings"));

           

            services.AddDbContext<BioSPEAKdbContext>(x => x.UseMySQL(Configuration.GetSection("DBSettings")["MySQLconn"]));
            services.AddScoped<IServiceUoW, BioSPEAKdbContext>();
            

            services.AddIdentity<UserAccount, Role>()
                .AddUserStore<UserStore>()
                .AddRoleStore<RoleStore>();

            services.AddScoped<ActivityService>();
            services.AddScoped<AssayService>();
            services.AddScoped<BioSampleService>();
            services.AddScoped<CVtermService>();
            services.AddScoped<DataExplorerService>();
            services.AddScoped<DatasetService>();
            services.AddScoped<ExportService>();
            services.AddScoped<FileService>();
            
            services.AddScoped<ProjectService>();
            services.AddScoped<SDTMreader>();
            services.AddScoped<StudyService>();
            
            services.AddScoped<TemplateService>();
            services.AddScoped<UserDatasetService>();
            services.AddScoped<UserAccountService>();
            services.AddScoped<CheckoutService>();
            services.AddScoped<QueryService>();
            services.AddScoped<SubjectLoader>();
            services.AddScoped<BioSampleLoader>();
            services.AddScoped<DataMatrixLoader>();
            services.AddScoped<ObservationLoader>();

            services.AddAuthorization(auth =>
            {
                auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser().Build());
            });

            services.AddMvc(config =>
            {
                var policy = new AuthorizationPolicyBuilder()
                                 .RequireAuthenticatedUser()
                                 .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            })
            //services.AddMvc();
            .AddJsonOptions(opts =>
                 {
                     // Force Camel Case to JSON
                     opts.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                     //opts.SerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
                 });

            BsonClassMap.RegisterClassMap<eTRIKS.Commons.Core.Domain.Model.ObservationModel.Observation>();
            BsonClassMap.RegisterClassMap<eTRIKS.Commons.Core.Domain.Model.ObservationModel.ObservedPropertyValue>();
            BsonClassMap.RegisterClassMap<eTRIKS.Commons.Core.Domain.Model.ObservationModel.CategoricalValue>();
            BsonClassMap.RegisterClassMap<eTRIKS.Commons.Core.Domain.Model.ObservationModel.OrdinalValue>();
            BsonClassMap.RegisterClassMap<eTRIKS.Commons.Core.Domain.Model.ObservationModel.NumericalValue>();
            BsonClassMap.RegisterClassMap<eTRIKS.Commons.Core.Domain.Model.ObservationModel.IntervalValue>();
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseCors("CorsPolicy");

            app.UseDeveloperExceptionPage();

            app.UseApplicationInsightsRequestTelemetry();

            app.UseApplicationInsightsExceptionTelemetry();

            var tokenIssuerOptions = Configuration.GetSection("TokenAuthOptions");
            var signingKey = new RsaSecurityKey(RSAKeyHelper.GenerateKey());
            var options = new TokenAuthOptions()
            {
                Audience = tokenIssuerOptions["Audience"],
                Issuer = tokenIssuerOptions["Issuer"],
                Endpoint = tokenIssuerOptions["Endpoint"],
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.RsaSha256Signature),
            };


            //Token Generation
            app.UseMiddleware<TokenProviderMiddleware>(Options.Create(options));

            //Token Bearer Validation 
            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                TokenValidationParameters =
                {
                    IssuerSigningKey = signingKey,
                    ValidAudience = tokenIssuerOptions["Audience"],
                    ValidIssuer = tokenIssuerOptions["Issuer"],
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(0)
                }
            });

            app.UseIdentity();

            // Configure the HTTP request pipeline.
            //app.UseStaticFiles();

            app.UseMvc();
        }
    }
}

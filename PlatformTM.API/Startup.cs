using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson.Serialization;
using MySql.Data.MySqlClient;
using MySQL.Data.Entity.Extensions;
using Newtonsoft.Json.Serialization;
using PlatformTM.API.Auth;
using PlatformTM.Core.Application.AccountManagement;
using PlatformTM.Core.Domain.Interfaces;
using PlatformTM.Core.Domain.Model.ObservationModel;
using PlatformTM.Core.Domain.Model.Users.Queries;
using PlatformTM.Data;
using PlatformTM.Data.Configuration;
using PlatformTM.Services.Configuration;
using PlatformTM.Services.DTOs.Explorer;
using PlatformTM.Services.Services;
using PlatformTM.Services.Services.HelperService;
using PlatformTM.Services.Services.Loading.AssayData;
using PlatformTM.Services.Services.Loading.SDTM;
using PlatformTM.Services.Services.OntologyManagement;
using PlatformTM.Services.Services.UserManagement;
using Swashbuckle.AspNetCore.Swagger;

namespace PlatformTM.API
{
    public class Startup
    {
        private IConfigurationRoot Configuration { get; }
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

            BsonClassMap.RegisterClassMap<ObservationNode>();
            BsonClassMap.RegisterClassMap<GroupNode>();
            BsonClassMap.RegisterClassMap<MedDRAGroupNode>();
            BsonClassMap.RegisterClassMap<MissingValue>();




            services.AddDbContext<PlatformTMdbContext>(x => x.UseMySQL(Configuration.GetSection("DBSettings")["MySQLconn"]));
            services.AddScoped<IServiceUoW, PlatformTMdbContext>();
            

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
            services.AddScoped<CacheService>();
            services.AddScoped<OLSclient>();
            services.AddScoped<OLSresultHandler>();

            services.AddScoped<SubjectLoader>();
            services.AddScoped<BioSampleLoader>();
            services.AddScoped<DataMatrixLoader>();
            services.AddScoped<HDloader>();
            services.AddScoped<ObservationLoader>();

            services.AddScoped<Formatter>();
            services.AddSingleton<Data.DbInitializer>();

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
            .AddJsonOptions(opts =>
                 {
                     // Force Camel Case to JSON
                     opts.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                     //opts.SerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
                 });

            BsonClassMap.RegisterClassMap<Observation>();
            BsonClassMap.RegisterClassMap<ObservedPropertyValue>();
            BsonClassMap.RegisterClassMap<CategoricalValue>();
            BsonClassMap.RegisterClassMap<OrdinalValue>();
            BsonClassMap.RegisterClassMap<NumericalValue>();
            BsonClassMap.RegisterClassMap<IntervalValue>();
            BsonClassMap.RegisterClassMap<ObservationQuery>();

			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new Info { Title = "PlatformTM API", Version = "v1" });
			});

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseCors("CorsPolicy");

            if(env.IsDevelopment())
            app.UseDeveloperExceptionPage();

            app.UseApplicationInsightsRequestTelemetry();

            app.UseApplicationInsightsExceptionTelemetry();

			// Enable middleware to serve generated Swagger as a JSON endpoint.
			app.UseSwagger();

			// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/api/v1/swagger/v1/swagger.json", "PlatformTM API V1");
			});

            //Token Generation
            var tokenIssuerOptions = Configuration.GetSection("TokenAuthOptions");
            var signingKey = new RsaSecurityKey(RSAKeyHelper.GenerateKey());
            var options = new TokenAuthOptions()
            {
                Audience = tokenIssuerOptions["Audience"],
                Issuer = tokenIssuerOptions["Issuer"],
                Endpoint = tokenIssuerOptions["Endpoint"],
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.RsaSha256Signature),
            };
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

            app.UseMvc();

        }
    }
}

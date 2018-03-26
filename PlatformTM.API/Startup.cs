using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson.Serialization;
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
using PlatformTM.Services.Services.UserManagement;
using Swashbuckle.AspNetCore.Swagger;

namespace PlatformTM.API
{
    public class Startup
    {
        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });
            services.AddOptions();



            services.AddAuthentication(o=>o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(cfg =>{
                    //CHANGE TO TRUE FOR PRODUCTION
                    cfg.RequireHttpsMetadata = false;
                    cfg.SaveToken = true;
                    cfg.TokenValidationParameters = new TokenValidationParameters(){
                        ValidIssuer = Configuration["TokenAuthOptions:Issuer"],
                        ValidAudience = Configuration["TokenAuthOptions:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["TokenAuthOptions:HmacSecretKey"]))
                    };
                });

            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy(
            //        "CanAccessAdminArea",
            //        policyBuilder => policyBuilder.RequireRole("ADMIN", "OWNER"));
            //    options.AddPolicy(
            //       "CanImportData",
            //       policyBuilder => policyBuilder.RequireClaim("CanImportData"));
            //});

            services.AddAuthorization(auth =>
            {
                auth.AddPolicy("Bearer", policy =>
                {
                    policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireAuthenticatedUser();
                });
            });


            services.Configure<DataAccessSettings>(Configuration.GetSection("DBSettings"));
            services.Configure<FileStorageSettings>(Configuration.GetSection("FileStorageSettings"));
            services.Configure<TokenAuthOptions>(Configuration.GetSection("TokenAuthOptions"));

            #region DI Registration
            services.AddDbContext<PlatformTMdbContext>
                    (x => x.UseMySql(Configuration.GetSection("DBSettings")["MySQLconn"], sqlOptions =>{
                         sqlOptions.EnableRetryOnFailure(7, System.TimeSpan.FromSeconds(5), null);}));
            services.AddScoped<IServiceUoW, PlatformTMdbContext>();

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

            services.AddScoped<SubjectLoader>();
            services.AddScoped<BioSampleLoader>();
            services.AddScoped<DataMatrixLoader>();
            services.AddScoped<HDloader>();
            services.AddScoped<ObservationLoader>();

            services.AddScoped<Formatter>();
            services.AddScoped<Data.DbInitializer>();
            services.AddScoped<JwtProvider>();
            #endregion

            #region MongoDB Class Mapppings
            BsonClassMap.RegisterClassMap<ObservationNode>();
            BsonClassMap.RegisterClassMap<GroupNode>();
            BsonClassMap.RegisterClassMap<MedDRAGroupNode>();
            BsonClassMap.RegisterClassMap<MissingValue>();
            BsonClassMap.RegisterClassMap<Observation>();
            BsonClassMap.RegisterClassMap<ObservedPropertyValue>();
            BsonClassMap.RegisterClassMap<CategoricalValue>();
            BsonClassMap.RegisterClassMap<OrdinalValue>();
            BsonClassMap.RegisterClassMap<NumericalValue>();
            BsonClassMap.RegisterClassMap<IntervalValue>();
            BsonClassMap.RegisterClassMap<ObservationQuery>();
            #endregion

            services.AddIdentity<UserAccount, Role>()
            .AddUserStore<UserStore>()
            .AddRoleStore<RoleStore>();

            services.AddMvc(config =>
            {
                //add a global filter so that requests are Authorized by default
                config.Filters.Add(new AuthorizeFilter("Bearer"));
            })
            .AddJsonOptions(opts =>{
                // Force Camel Case to JSON
                opts.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                //opts.SerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
            });


			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new Info { Title = "PlatformTM API", Version = "v1" });
			});

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UsePathBase("/api/v1/");

            app.UseCors("CorsPolicy");

            if(env.IsDevelopment())
                app.UseDeveloperExceptionPage();

			// Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger()
               .UseSwaggerUI(c=> {
                c.SwaggerEndpoint("/api/v1/swagger/v1/swagger.json", "PlatformTM API V1");
               });

            app.UseAuthentication();

            app.UseMvc();

        }
    }
}

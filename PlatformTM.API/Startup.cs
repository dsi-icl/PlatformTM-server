using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.Serialization;
using PlatformTM.API.Auth;
using PlatformTM.Core.Application.AccountManagement;
using PlatformTM.Core.Domain.Interfaces;
using PlatformTM.Core.Domain.Model.ObservationModel;
using PlatformTM.Core.Domain.Model.Users.Queries;
using PlatformTM.Data;
using PlatformTM.Data.Configuration;
using PlatformTM.Models.Configuration;
using PlatformTM.Models.DTOs.Explorer;
using PlatformTM.Models.Services;
using PlatformTM.Models.Services.HelperService;
using PlatformTM.Models.Services.Loading.AssayData;
using PlatformTM.Models.Services.Loading.SDTM;
using PlatformTM.Models.Services.UserManagement;
using Swashbuckle.AspNetCore.Swagger;
using Newtonsoft.Json.Serialization;
using Microsoft.OpenApi.Models;

namespace PlatformTM.API
{
    public class Startup
    {
        private IConfiguration Configuration { get; }
        private readonly IWebHostEnvironment _env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.SetIsOriginAllowed(origin => true)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials().WithExposedHeaders("Content-Disposition"));
                
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
            services.Configure<AuthMessageSenderOptions>(Configuration);

            #region DI Registration
            var serverVersion = ServerVersion.AutoDetect(Configuration.GetSection("DBSettings")["MySQLconn"]);
            services.AddDbContext<PlatformTMdbContext>
                    (x => x.UseMySql(Configuration.GetSection("DBSettings")["MySQLconn"], serverVersion, sqlOptions =>{
                         sqlOptions.EnableRetryOnFailure(7, System.TimeSpan.FromSeconds(5), null);}));
            services.AddScoped<IServiceUoW, PlatformTMdbContext>();

            services.AddScoped<ActivityService>();
            services.AddScoped<AssayService>();
            services.AddScoped<BioSampleService>();
            services.AddScoped<CVtermService>();
            services.AddScoped<DataExplorerService>();
            services.AddScoped<DatasetService>();
            services.AddScoped<DatasetDescriptorService>();
            services.AddScoped<ExportService>();
            services.AddScoped<FileService>();

            services.AddScoped<ProjectService>();
            services.AddScoped<SDTMreader>();
            services.AddScoped<StudyService>();
            
            services.AddScoped<TemplateService>();
            services.AddScoped<AnalysisDatasetService>();
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
            services.AddScoped<EmailService>();
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

            services.AddIdentity<UserAccount, Role>(config =>
            {
                config.SignIn.RequireConfirmedEmail = false;
            })
            .AddUserStore<UserStore>()
            .AddRoleStore<RoleStore>()
            .AddDefaultTokenProviders();

            services.AddResponseCompression(options => {
                options.EnableForHttps = true;
            });

            services.AddControllers().AddNewtonsoftJson();

            services.AddMvc(config =>
            {
                //add a global filter so that requests are Authorized by default
                config.Filters.Add(new AuthorizeFilter("Bearer"));
            }).AddNewtonsoftJson
            (opts =>{
                // Force Camel Case to JSON
                opts.SerializerSettings.ContractResolver = new  CamelCasePropertyNamesContractResolver();
                opts.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                //opts.SerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
            });

            services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "PlatformTM API", Version = "v1" });
			});

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConfiguration(Configuration.GetSection("Logging"));
                loggingBuilder.AddConsole();
                loggingBuilder.AddDebug();
            });

        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            

            app.UsePathBase("/api/v1/");

            app.UseResponseCompression();

            app.UseRouting();
            app.UseCors("CorsPolicy");

            app.UseAuthentication();
            //app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });


            

            if(_env.IsDevelopment())
                app.UseDeveloperExceptionPage();

			// Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger()
               .UseSwaggerUI(c=> {
                c.SwaggerEndpoint("/api/v1/swagger/v1/swagger.json", "PlatformTM API V1");
               });


            //app.UseMvc();

        }
    }
}

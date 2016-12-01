using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        

        // This method gets called by the runtime. Use this method to add services to the container
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
            
            services.Configure<DataAccessSettings>(Configuration.GetSection("DBSettings"));

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
            services.AddScoped<ObservationService>();
            services.AddScoped<ProjectService>();
            services.AddScoped<SDTMreader>();
            services.AddScoped<StudyService>();
            services.AddScoped<SubjectService>();
            services.AddScoped<TemplateService>();
            services.AddScoped<UserDatasetService>();
            services.AddScoped<UserAccountService>();

            services.AddMvc();

           
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

            app.UseIdentity();

            app.UseMvc();
        }
    }
}

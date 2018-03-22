using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PlatformTM.Data;

namespace PlatformTM.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            if(EnvironmentName.Development != Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"))
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var dbContext = services.GetRequiredService<PlatformTMdbContext>();
                        dbContext.InitDB();
                    var dbInitializer = services.GetRequiredService<Data.DbInitializer>();
                        dbInitializer.SeedDB();

                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex.Message, "An error occurred while seeding the database.");
                }
            }

            host.Run();
        }
    }
}

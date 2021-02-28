using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NewsCrawler.Persistence.Postgres;
using System;
using System.Threading.Tasks;

namespace NewsCrawler.WebUI
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            var webHost = CreateWebHostBuilder(args).Build();

            using (var sp_scope = webHost.Services.CreateScope())
            {
                var config = webHost.Services.GetRequiredService<IConfiguration>();
                string pgString = config.GetConnectionString("PostgresNewsArticleDatabase");
                Console.WriteLine($"Connecting to '{pgString}'.");
                var context = sp_scope.ServiceProvider.GetRequiredService<PostgresNewsArticleContext>();
                Console.WriteLine("Checking if database is migrated.");
                bool created = await context.Database.EnsureCreatedAsync();
                if (created)
                {
                    Console.WriteLine("Database migrated.");
                }
            }
            await webHost.RunAsync();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args).ConfigureAppConfiguration(config =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                    .AddEnvironmentVariables();
            })
            .UseStartup<Startup>();
        }
    }
}

using DocumentScanner;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NewsCrawler.Persistence;
using NewsCrawler.Persistence.Postgres;

namespace NewsCrawler.WebUI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<NewsArticleContext>(options => options.UseSqlServer(
                Configuration.GetConnectionString("NewsArticleDatabase"),
                sqlServerOptions => sqlServerOptions.CommandTimeout(120)));
            services.AddDbContext<PostgresNewsArticleContext>(options => options.UseNpgsql(
                Configuration.GetConnectionString("PostgresNewsArticleDatabase"),
                contextOptions => contextOptions.CommandTimeout(120 * 2)),
                ServiceLifetime.Transient);
            services.AddTransient<DocumentScannerService>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "detail",
                    defaults: new {
                        controller = "ArticleDetail",
                        action = "Index" },
                    template: "Detail/{id}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}

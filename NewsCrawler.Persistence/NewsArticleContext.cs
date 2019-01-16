using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace NewsCrawler.Persistence
{
    public class NewsArticleContext : DbContext
    {
        public DbSet<Article> Articles { get; set; }

        public NewsArticleContext(DbContextOptions<NewsArticleContext> options)
            :base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            var config = builder.Build();


            var connectionString = config.GetConnectionString("NewsArticleDatabase");
            //optionsBuilder.UseSqlServer(connectionString);
        }
    }
}

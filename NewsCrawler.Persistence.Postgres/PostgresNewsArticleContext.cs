using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace NewsCrawler.Persistence.Postgres
{
    public class PostgresNewsArticleContext : DbContext
    {
        public DbSet<Article> Articles { get; set; }

        public DbSet<WordCount> WordCount { get; set; }

        public DbSet<Source> Source { get; set; }

        public PostgresNewsArticleContext(DbContextOptions<PostgresNewsArticleContext> options)
            :base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Article>();

            modelBuilder.Entity<WordCount>();

            modelBuilder.Entity<Source>();

            modelBuilder.Entity<ArticleContent>();

            modelBuilder.Entity<ArticleCleanedContent>();
        }
    }
}

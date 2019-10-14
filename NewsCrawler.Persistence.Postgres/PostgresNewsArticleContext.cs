﻿using Microsoft.EntityFrameworkCore;
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
            modelBuilder.Entity<Article>()
                .Property(a => a.NewsSource)
                .HasDefaultValue("Unspecified");

            modelBuilder.Entity<WordCount>();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
            var config = builder.Build();

            var connectionString = config.GetConnectionString("PostgresNewsArticleDatabase");
            optionsBuilder.UseNpgsql(connectionString);
        }
    }
}

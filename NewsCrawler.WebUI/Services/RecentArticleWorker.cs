using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NewsCrawler.Persistence.Postgres;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NewsCrawler.WebUI.Services
{
    public class RecentArticleWorker : BackgroundService
    {
        private readonly ILogger _logger;

        private readonly TimeSpan UpdateDelay = new TimeSpan(0, 30, 0);

        public IServiceProvider Services { get; }


        public RecentArticleWorker(IServiceProvider services,
            ILogger<RecentArticleWorker> logger)
        {
            Services = services;
            _logger = logger;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                "Consume Scoped Service Hosted Service running.");

            while (!stoppingToken.IsCancellationRequested)
            {
                await DoWork();
                await Task.Delay(UpdateDelay, stoppingToken);
            }
        }

        private async Task DoWork()
        {
            _logger.LogInformation(
                "Consume Scoped Service Hosted Service is working.");

            using (var scope = Services.CreateScope())
            {
                var scopedProcessingService =
                    scope.ServiceProvider
                        .GetRequiredService<PostgresNewsArticleContext>();
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                "Consume Scoped Service Hosted Service is stopping.");

            await Task.CompletedTask;
        }
    }
}

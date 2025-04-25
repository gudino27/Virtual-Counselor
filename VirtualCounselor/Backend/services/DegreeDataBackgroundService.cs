

namespace BlazorApp1.Services
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using VirtualCounselor.Backend;

    /// <summary>
    /// On startup, calls DegreeScrape.scrapeall(), then caches DegreeScrape.degreeList.
    /// </summary>
    public class DegreeDataBackgroundService : BackgroundService
    {
        private readonly ILogger<DegreeDataBackgroundService> _logger;
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _refresh = TimeSpan.FromHours(1);
        private const string DegreeDataCacheKey = "DegreeData";

        public DegreeDataBackgroundService(
            ILogger<DegreeDataBackgroundService> logger,
            IMemoryCache cache)
        {
            _logger = logger;
            _cache = cache;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Initial load
            await RefreshDegrees();

            // Repeat hourly (optional)
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(_refresh, stoppingToken);
                await RefreshDegrees();
            }
        }

        private Task RefreshDegrees()
        {
            try
            {
                _logger.LogInformation("Starting degree scrape...");
                // static scraper populates DegreeScrape.degreeList
                DegreeScrape.scrapeall();

                // write into cache
                if (DegreeScrape.degreeList != null && DegreeScrape.degreeList.Any())
                {
                    _cache.Set(DegreeDataCacheKey, DegreeScrape.degreeList);
                    _logger.LogInformation("DegreeDataBackgroundService is running...");
                    _logger.LogInformation($"Cached {DegreeScrape.degreeList.Count} degrees.");
                }
                else
                {
                    _logger.LogWarning("DegreeScrape.degreeList is empty or null.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error scraping degrees");
            }
            return Task.CompletedTask;
        }
    }
}

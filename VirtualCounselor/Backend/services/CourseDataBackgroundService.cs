﻿namespace BlazorApp1.Services
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    public class CourseDataBackgroundService : BackgroundService
    {
        private readonly ILogger<CourseDataBackgroundService> _logger;
        private readonly IMemoryCache _cache;
        private const string CourseDataCacheKey = "CourseData";
        private readonly TimeSpan _refreshInterval = TimeSpan.FromHours(1);
        private readonly CourseService _courseService;
        private readonly HttpClient _httpClient;

        // Added CourseService for constructor parameter
        public CourseDataBackgroundService(ILogger<CourseDataBackgroundService> logger, IMemoryCache cache, CourseService courseService, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _cache = cache;
            _courseService = courseService;
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.Timeout = TimeSpan.FromMinutes(5); // Increase timeout so the HttpClient have enough time to make requests to the WebDriver server
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //try
            //{
            //    Console.WriteLine("Initial course data load starting.");
            //    await Task.Run(() => Sprint4.Runall(), stoppingToken);
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine($"An error occurred while loading course data: {ex}");
            //}

            try
            {
                // Run the initial thing when the app starts
                _logger.LogInformation("Initial course data load starting.");
                await Task.Run(() => Sprint4.Runall(), stoppingToken);
                UpdateCache();
                var courses = Sprint4.CampusesList
                            .SelectMany(c => c.Terms)
                            .SelectMany(t => t.Courses)
                            .ToList();
                _courseService.ScrapedCourses = courses;
                _logger.LogInformation($"Updated CourseService scraped courses count: {courses.Count}");
                _logger.LogInformation("Initial course data load complete.");

                //  refresh data hourly
                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(_refreshInterval, stoppingToken);

                    _logger.LogInformation("Refreshing course data...");
                    await Task.Run(() => Sprint4.DateLoad(), stoppingToken);
                    UpdateCache();
                    _logger.LogInformation("Course data refreshed.");
                }
            }
            catch (TaskCanceledException)
            {
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while loading course date");
            }
        }

        private void UpdateCache()
        {
            _cache.Set(CourseDataCacheKey, Sprint4.CampusesList);
        }

        //public override void Dispose()
        //{
        //    _httpClient?.Dispose();
        //    base.Dispose();
        //}
    }
}

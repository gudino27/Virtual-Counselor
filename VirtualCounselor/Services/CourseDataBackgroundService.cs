namespace BlazorApp1.Services
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
        private const string DegreeDataCacheKey = "DegreeData";
        private readonly TimeSpan _refreshInterval = TimeSpan.FromHours(1);
        private readonly CourseService _courseService;

        // Added CourseService for constructor parameter
        public CourseDataBackgroundService(ILogger<CourseDataBackgroundService> logger, IMemoryCache cache, CourseService courseService)
        {
            _logger = logger;
            _cache = cache;
            _courseService = courseService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation("Initial course data load starting.");
                await Task.Run(() => CourseScrape.Runall(), stoppingToken);
                UpdateCache();
                var courses = CourseScrape.CampusesList
                            .SelectMany(c => c.Terms)
                            .SelectMany(t => t.Courses)
                            .ToList();
                _courseService.ScrapedCourses = courses;
                _logger.LogInformation($"Updated CourseService scraped courses count: {courses.Count}");
                _logger.LogInformation("Initial course data load complete.");
                _logger.LogInformation("Initial degree data load starting.");
                // Run the initial thing when the app starts
                await Task.Run(() => DegreeScrape.scrapeall(), stoppingToken);
                updateCacheDegree();
                _logger.LogInformation("Initial degree data load complete.");
                await Task.Delay(1000, stoppingToken); // Delay to ensure degree data is loaded before course data

                //  refresh data hourly
                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(_refreshInterval, stoppingToken);

                    _logger.LogInformation("Refreshing course data...");
                    await Task.Run(() => CourseScrape.DateLoad(), stoppingToken);
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
            _cache.Set(CourseDataCacheKey, CourseScrape.CampusesList);
        }
        private void updateCacheDegree()
        {
            _cache.Set(DegreeDataCacheKey, DegreeScrape.degreeList);
        }
    }
}

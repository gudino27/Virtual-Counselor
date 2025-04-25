

namespace BlazorApp1.Services
{
    using System.Collections.Generic;
    using Microsoft.Extensions.Caching.Memory;
    using VirtualCounselor;
    using VirtualCounselor.Backend;

    /// <summary>
    /// Exposes the scraped Majors/Minors stored in IMemoryCache under "DegreeData".
    /// </summary>
    public class DegreeService
    {
        private readonly IMemoryCache _cache;
        private const string DegreeDataCacheKey = "DegreeData";

        public DegreeService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public List<Degree> GetAllMajors()
        {
            if (_cache.TryGetValue(DegreeDataCacheKey, out List<Degree> degrees) && degrees != null)
            {
                Console.WriteLine($"Degrees retrieved from cache: {degrees.Count}");
                return degrees;
            }
            else
            {
                // Log a warning if the cache is empty
                Console.WriteLine("Warning: Degree data cache is empty.");
                return new List<Degree>(); // So it returns an empty list if no data is available
            }
        }

        public Degree? GetDegreeById(int id)
        {
            if (_cache.TryGetValue(DegreeDataCacheKey, out List<Degree> degrees) && degrees != null)
            {
                return degrees.FirstOrDefault(d => d.Id == id);
            }
            return null;
        }

        public List<Degree> SearchDegrees(string searchTerm)
        {
            if (_cache.TryGetValue(DegreeDataCacheKey, out List<Degree> degrees) && degrees != null)
            {
                return degrees
                    .Where(d => d.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            return new List<Degree>();
        }

        public void RefreshDegrees()
        {
            DegreeScrape.scrapeall(); // Call the scraping logic
            var degrees = DegreeScrape.degreeList; // Access the scraped data
            _cache.Set(DegreeDataCacheKey, degrees);
        }

    }
}

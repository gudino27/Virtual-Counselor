using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;

namespace BlazorApp1.Services
{
    public class DegreeService
    {
        private readonly IMemoryCache _cache;
        private const string DegreeDataCacheKey = "DegreeData";

        public DegreeService(IMemoryCache cache)
        {
            _cache = cache;
        }

        List<Major> Majors =>
            _cache.Get<List<Degree>>(DegreeDataCacheKey)?
                  .OfType<Major>()
                  .ToList()
            ?? new();

        public List<Major> GetAllMajors() => Majors;

        /// <summary>Returns null if not found.</summary>
        public string? GetCredit(string majorName, string courseName)
        {
            var m = Majors.FirstOrDefault(x => x.DegreeDescription == majorName);
            return m?.CourseRequirements
                      .FirstOrDefault(c => c.Name == courseName)
                      ?.Credits;
        }
    }
}
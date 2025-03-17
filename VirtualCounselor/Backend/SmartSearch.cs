using System;
using System.Collections.Generic;
using System.Linq;

namespace VirtualCounselor.Backend
{
    /// <summary>
    /// SmartSearch provides keyword-based search across all courses
    /// and recommends UCORE courses not yet taken based on selected degrees.
    /// </summary>
    public class SmartSearch
    {
        private readonly CourseManager courseManager;
        private readonly List<Degree> activeDegrees;
        private readonly HashSet<string> takenCourses;

        public SmartSearch(CourseManager courseMgr, List<Degree> selectedDegrees, List<string> takenCoursesList)
        {
            courseManager = courseMgr ?? throw new ArgumentNullException(nameof(courseMgr));
            activeDegrees = selectedDegrees ?? throw new ArgumentNullException(nameof(selectedDegrees));
            takenCourses = new HashSet<string>(takenCoursesList ?? throw new ArgumentNullException(nameof(takenCoursesList)));
        }

        /// <summary>
        /// Searches for courses based on a keyword.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<Course> SearchCourses(string query)
        {
            Console.WriteLine($"SmartSearch.SearchCourses called with query: '{query}'");

            if (string.IsNullOrWhiteSpace(query))
            {
                Console.WriteLine("Query is empty, returning empty list");
                return new List<Course>();
            }

            var keywords = query
                .ToLower()
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            Console.WriteLine($"Searching with keywords: {string.Join(", ", keywords)}");
            Console.WriteLine($"Total courses available: {courseManager.GetAllCourses().Count}");

            var results = courseManager.GetAllCourses()
                .Where(course => !takenCourses.Contains(course.CourseCode) &&
                    keywords.All(k =>
                        course.CourseCode.ToLower().Contains(k) ||
                        course.Title.ToLower().Contains(k) ||
                        course.Prefix.ToLower().Contains(k)
                    ))
                .ToList();

            Console.WriteLine($"Found {results.Count} matching courses");
            foreach (var course in results)
            {
                Console.WriteLine($"Match found: {course.CourseCode} - {course.Title}");
            }

            return results;
        }

        /// <summary>
        /// Recommends UCORE courses that are not yet taken.
        /// </summary>
        /// <returns></returns>
        public List<Course> RecommendUCoreCourses()
        {
            var allCourses = courseManager.GetAllCourses();
            var requiredUCoreCategories = GetRequiredUCoreCategories();
            var completedUCore = allCourses
                .Where(c => c.UCoreCategory != null && takenCourses.Contains(c.CourseCode))
                .Select(c => c.UCoreCategory)
                .ToHashSet();

            var unmetUCore = requiredUCoreCategories.Except(completedUCore);

            return allCourses
                .Where(c =>
                    c.UCoreCategory != null &&
                    unmetUCore.Contains(c.UCoreCategory) &&
                    !takenCourses.Contains(c.CourseCode))
                .ToList();
        }

        /// <summary>
        /// Recommends courses required for the selected degrees that are not yet taken.
        /// </summary>
        /// <returns></returns>
        public List<string> RecommendUnmetDegreeCourses()
        {
            var required = new HashSet<string>();

            foreach (var degree in activeDegrees)
            {
                var degreeCourses = degree.GetRequiredCourses();
                foreach (var course in degreeCourses)
                    required.Add(course);
            }

            return required.Except(takenCourses).ToList();
        }

        /// <summary>
        /// Gets all recommended courses (UCORE + Major/Minor courses).
        /// </summary>
        /// <returns></returns>
        public List<Course> GetAllRecommendedCourses()
        {
            var recommendations = new List<Course>();
            var unmetCodes = RecommendUnmetDegreeCourses();
            var allCourses = courseManager.GetAllCourses();

            foreach (var code in unmetCodes)
            {
                var course = courseManager.GetCourse(code);
                if (course != null && !takenCourses.Contains(code))
                    recommendations.Add(course);
            }

            var ucoreCourses = RecommendUCoreCourses();
            foreach (var course in ucoreCourses)
            {
                if (!recommendations.Any(c => c.CourseCode == course.CourseCode))
                {
                    recommendations.Add(course);
                }
            }

            return recommendations
                .Distinct()
                .OrderBy(c => c.CourseCode)
                .ToList();
        }

        private HashSet<string> GetRequiredUCoreCategories()
        {
            return new HashSet<string>
            {
                "WRTG", "COMM", "QUAN", "SSCI", "HUM", "DIVR", "PSCI", "CAPS"
            };
        }
    }
}

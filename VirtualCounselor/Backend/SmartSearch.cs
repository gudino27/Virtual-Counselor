// File: VirtualCounselor/Backend/SmartSearch.cs
using System;
using System.Collections.Generic;
using System.Linq;

namespace VirtualCounselor.Backend
{
    /// <summary>
    /// Provides keyword search for degrees & courses, plus recommendations.
    /// </summary>
    public class SmartSearch
    {
        private readonly CourseManager courseManager;
        private readonly List<Degree> degreeCatalog;
        public List<Degree> ActiveDegrees { get; }
        private readonly HashSet<string> takenCourses;

        public SmartSearch(CourseManager cm)
        {
            courseManager = cm ?? throw new ArgumentNullException(nameof(cm));
            degreeCatalog = DegreeManager.GetDegreeList();
            ActiveDegrees = new List<Degree>();
            takenCourses = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            InitializePredefinedCourses();
        }

        /// <summary>Seed a handful of courses so SearchCourses has data.</summary>
        private void InitializePredefinedCourses()
        {
            var predefined = new List<Course>
            {
                new() { CourseCode="CPT S 101", Title="Intro to CS",             Credits=1 },
                new() { CourseCode="CPT S 121", Title="Program Design",           Credits=4 },
                new() { CourseCode="ENGLISH 101", Title="College Composition",    Credits=3 },
                new Course { CourseCode = "MATH 171", Title = "Calculus I", Credits = 4 },
                new Course { CourseCode = "PHIL 201", Title = "Introduction to Philosophy", Credits = 3 },
                new Course { CourseCode = "CPT S 122", Title = "Data Structures", Credits = 4 },
                new Course { CourseCode = "HISTORY 105", Title = "Roots of Contemporary Issues", Credits = 3 },
                new Course { CourseCode = "MATH 172", Title = "Calculus II", Credits = 4 },
                new Course { CourseCode = "MATH 216", Title = "Discrete Structures", Credits = 3 },
                new Course { CourseCode = "CPT S 223", Title = "Advanced Data Structures", Credits = 3 },
                new Course { CourseCode = "CPT S 260", Title = "Systems Programming", Credits = 3 },
                new Course { CourseCode = "MATH 220", Title = "Linear Algebra", Credits = 2 },
                new Course { CourseCode = "MATH 273", Title = "Calculus III", Credits = 2 },
                new Course { CourseCode = "CPT S 317", Title = "Software Design", Credits = 3 },
                new Course { CourseCode = "CPT S 322", Title = "Software Engineering Principles", Credits = 3 },
                new Course { CourseCode = "CPT S 355", Title = "Database Systems", Credits = 3 },
                new Course { CourseCode = "CPT S 302", Title = "Operating Systems", Credits = 3 },
                new Course { CourseCode = "CPT S 327", Title = "Computer Networks", Credits = 3 },
                new Course { CourseCode = "CPT S 350", Title = "Software Testing", Credits = 3 },
                new Course { CourseCode = "CPT S 360", Title = "Computer Architecture", Credits = 4 },
                new Course { CourseCode = "ENGLISH 402", Title = "Technical Writing", Credits = 3 },
                new Course { CourseCode = "STAT 360", Title = "Probability and Statistics", Credits = 3 },
                new Course { CourseCode = "CPT S 421", Title = "Compiler Design", Credits = 3 },
                new Course { CourseCode = "CPT S 423", Title = "Capstone Project", Credits = 3 }
                // I might be adding more later
            };

            foreach (var c in predefined)
            {
                courseManager.AddCourse(c);
            }
        }

        /// <summary>Search degrees by name (autocomplete bucket).</summary>
        public List<Degree> SearchDegrees(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<Degree>();

            query = query.Trim().ToLower();
            return degreeCatalog
                .Where(d => d.Name.ToLower().Contains(query))
                .ToList();
        }

        /// <summary>Search courses by code/title, excluding already taken.</summary>
        public List<Course> SearchCourses(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<Course>();

            var keywords = query.ToLower()
                                .Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return courseManager.GetAllCourses()
                .Where(c => !takenCourses.Contains(c.CourseCode)
                         && keywords.All(k =>
                            c.CourseCode.ToLower().Contains(k)
                         || c.Title.ToLower().Contains(k)))
                .ToList();
        }

        /// <summary>Mark a course code as taken (e.g. from transcript).</summary>
        public void AddTakenCourse(string courseCode)
        {
            if (!string.IsNullOrWhiteSpace(courseCode))
                takenCourses.Add(courseCode);
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
        //public List<string> RecommendUnmetDegreeCourses()
        //{
        //    var required = new HashSet<string>();

        //    foreach (var degree in activeDegrees)
        //    {
        //        var degreeCourses = degree.GetRequiredCourses();
        //        foreach (var course in degreeCourses)
        //            required.Add(course);
        //    }

        //    return required.Except(takenCourses).ToList();
        //}

        /// <summary>
        /// Gets all recommended courses (UCORE + Major/Minor courses).
        /// </summary>
        /// <returns></returns>
        //public List<Course> GetAllRecommendedCourses()
        //{
        //    var recommendations = new List<Course>();
        //    var unmetCodes = RecommendUnmetDegreeCourses();
        //    var allCourses = courseManager.GetAllCourses();

        //    foreach (var code in unmetCodes)
        //    {
        //        var course = courseManager.GetCourse(code);
        //        if (course != null && !takenCourses.Contains(code))
        //            recommendations.Add(course);
        //    }

        //    var ucoreCourses = RecommendUCoreCourses();
        //    foreach (var course in ucoreCourses)
        //    {
        //        if (!recommendations.Any(c => c.CourseCode == course.CourseCode))
        //        {
        //            recommendations.Add(course);
        //        }
        //    }

        //    return recommendations
        //        .Distinct()
        //        .OrderBy(c => c.CourseCode)
        //        .ToList();
        //}

        private HashSet<string> GetRequiredUCoreCategories()
        {
            return new HashSet<string>
            {
                "WRTG", "COMM", "QUAN", "SSCI", "HUM", "DIVR", "PSCI", "CAPS"
            };
        }
    }
}

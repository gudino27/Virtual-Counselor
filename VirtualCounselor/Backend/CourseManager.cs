using System;
using System.Collections.Generic;
using System.Linq;

namespace VirtualCounselor
{
    /// <summary>
    /// CourseManager will handle all of the course data read in by WebScrapper.
    /// This class should manage storing and providing access to all data on all courses.
    /// With the class storing a list of Courses.
    /// </summary>
    public class CourseManager
    {
        private List<Course> courses = new List<Course> ();

        /// <summary>Adds a course to the bucket.</summary>
        public void AddCourse(Course course)
        {
            if (course == null) throw new ArgumentNullException(nameof(course));
            courses.Add(course);
        }

        /// <summary>All courses loaded (scraped or predefined).</summary>
        public List<Course> GetAllCourses()
            => courses.ToList();

        /// <summary>Find a single course by its code.</summary>
        public Course? GetCourse(string courseCode)
            => courses.FirstOrDefault(c =>
                string.Equals(c.CourseCode, courseCode, StringComparison.OrdinalIgnoreCase));
    }
}

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
        //private List<Course> courses = new List<Course> ();

        /// <summary>
        /// I switched to a dictionary instead of List for faster lookups by course code, which will also help in not needing to scan the list for duplicates manually since dictionary don't allow duplicates.
        /// I might however go back to List if I especially need to handle duplicate sections (I'm thinking for instance CS 121-01, CS 121-02, etc.).
        /// </summary>
        private Dictionary<string, Course> courses = new();

        public bool AddCourse(Course course)
        {
            if (course == null || string.IsNullOrWhiteSpace(course.CourseCode))
                return false;

            courses[course.CourseCode] = course;
            return true;
        }

        public Course GetCourse(string courseCode)
        {
            courses.TryGetValue(courseCode, out var course);
            return course;
        }

        public List<Course> GetAllCourses()
        {
            return courses.Values.ToList();
        }

        public void Clear()
        {
            courses.Clear();
        }
    }
}

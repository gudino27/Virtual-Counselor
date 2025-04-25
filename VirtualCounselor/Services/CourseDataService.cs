namespace BlazorApp1.Services
{
    using System.Collections.Generic;
    using System.Linq;
    public class CourseService
    {
        
        public List<CourseData> ScrapedCourses { get; set; } = new List<CourseData>();

        public List<CourseData> GetAllCourseData() =>
            ScrapedCourses.Where(c => !string.IsNullOrEmpty(c.CourseName)).ToList();
        // This copies data from CourseScrape.CampusesList into a property or returns it directly.
    public List<Campus> GetAllCampuses()
        {
            // Ensure CourseScrape.Runall() is called at least once in your application’s startup 
            // so that CampusesList is populated.
            return CourseScrape.CampusesList;
        }

        public List<Term> GetTermsForCampus(string campusName)
        {
            var campus = CourseScrape.CampusesList
                .FirstOrDefault(c => c.Name.Equals(campusName, StringComparison.OrdinalIgnoreCase));
            return campus?.Terms ?? new List<Term>();
        }

        public List<CourseData> GetCourses(string campusName, string termDescription)
        {
            var campus = CourseScrape.CampusesList
                .FirstOrDefault(c => c.Name.Equals(campusName, StringComparison.OrdinalIgnoreCase));
            var term = campus?.Terms.FirstOrDefault(t => t.Description == termDescription);
            return term?.Courses ?? new List<CourseData>();
        }
    }

}

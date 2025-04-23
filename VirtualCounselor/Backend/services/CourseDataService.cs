namespace BlazorApp1.Services
{
    /// <summary>
    /// This class serves as the access point for the front end to access stored courses in the cache.
    /// </summary>
    public class CourseService
    {
        
        public List<CourseData> ScrapedCourses { get; set; } = new List<CourseData>();

        public List<CourseData> GetAllCourseData() =>
            ScrapedCourses.Where(c => !string.IsNullOrEmpty(c.CourseName)).ToList();
    }
}

namespace BlazorApp1.Services
{
    public class CourseService
    {
        
        public List<CourseData> ScrapedCourses { get; set; } = new List<CourseData>();

        public List<CourseData> GetAllCourseData() =>
            ScrapedCourses.Where(c => !string.IsNullOrEmpty(c.CourseName)).ToList();
    }
}

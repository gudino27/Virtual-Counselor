namespace VirtualCounselor.Backend
{
    using System.Collections.Generic;
    using System.Linq;
    using BlazorApp1.Services;

    /// <summary>  
    /// Handles scraping and transforming degree-related data.  
    /// </summary>  
    public class DegreeScrape
    {
        // List to store scraped degrees  
        public static List<Degree> degreeList { get; private set; } = new List<Degree>();

        public static void scrapeall()
        {
            // Clear the existing degree list  
            degreeList.Clear();

            //degreeList = new List<Degree>
            //{
            //    new Degree("Computer Science", 1),
            //    new Degree("Mechanical Engineering", 2),
            //    new Degree("Business Administration", 3)
            //};

            Console.WriteLine("Cleared existing degree list.");

            // Use the existing scraping logic from webscrapecourses.cs  
            Sprint4.Runall();
            Console.WriteLine("Completed Sprint4.Runall().");

            // Transform the scraped course data into degree data  
            foreach (var campus in Sprint4.CampusesList)
            {
                foreach (var term in campus.Terms)
                {
                    // Group courses by degree (or major/minor)  
                    var groupedCourses = term.Courses
                        .GroupBy(c => c.Title) // Assuming Title represents the degree/major  
                        .ToList();

                    foreach (var group in groupedCourses)
                    {
                        // Create a new Degree object for each group  
                        var degree = new Degree(group.Key, degreeList.Count + 1);

                        // Add courses to the degree  
                        foreach (var course in group)
                        {
                            degree.AddRequiredCourse(new Course(
                                course.CourseName ?? string.Empty,
                                degree.Id,
                                course.Title ?? string.Empty,
                                string.Empty, // Placeholder for CourseCode  
                                0, // Placeholder for Credits  
                                null // Placeholder for UCoreCategory  
                            ));
                        }

                        // Add the degree to the degree list  
                        degreeList.Add(degree);
                    }
                }
            }
            // Log the number of degrees scraped
            Console.WriteLine($"Scraped {degreeList.Count} degrees.");
        }
    }
}

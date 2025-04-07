using VirtualCounselor.Backend;

namespace VirtualCounselor
{
    /// <summary>
    /// This class is going to be the main hub for the backend of our system.
    /// Any communication between frontend and backend will happen here. This means that this backend class will have coverage over the entirety of the backend.
    /// It'll serve as a container and communicator for the rest of the logic.
    /// </summary>
    public class CentralBackend
    {
        // The virtual conselor will contain the rest of the programs logic in itself so it can communicate with them (via function calls or events)
        private CourseManager courseManager = new CourseManager();
        private DegreeManager degreeManager = new DegreeManager();
        //private WebScraper webScraper = new WebScraper();
        //private Sprint4 webScraper;


        private SmartSearch smartSearch;
        private List<string> takenCourses = new();

        public CentralBackend()
        {
            //webScraper = new Sprint4(courseManager, degreeManager);
            InitializeSystem();
        }

        public CourseManager GetCourseManager() => courseManager;
        public DegreeManager GetDegreeManager() => degreeManager;

        // Initialize the student profile. I ideally want this called only after login/setup
        public void InitializeStudentData(List<string> taken, Major major, Minor minor = null, Major doubleMajor = null)
        {
            takenCourses = taken ?? new List<string>();

            degreeManager.CurrentMajor = major;
            degreeManager.CurrentMinor = minor;
            degreeManager.CurrentDoubleMajor = doubleMajor;

            var activeDegrees = degreeManager.GetActiveDegrees();

            smartSearch = new SmartSearch(courseManager, activeDegrees, takenCourses);
        }

        // Search courses using keywords
        public List<Course> SearchCourses(string query)
        {
            return smartSearch?.SearchCourses(query) ?? new List<Course>();
        }

        // Get all recommended UCORE courses
        public List<Course> GetRecommendedUCoreCourses()
        {
            return smartSearch?.RecommendUCoreCourses() ?? new List<Course>();
        }

        // Get required courses for the student’s degrees (not yet taken)
        public List<string> GetUnmetDegreeCourses()
        {
            return smartSearch?.RecommendUnmetDegreeCourses() ?? new List<string>();
        }

        // Get ALL recommended courses (UCORE + Major/Minor)
        public List<Course> GetAllRecommendedCourses()
        {
            return smartSearch?.GetAllRecommendedCourses() ?? new List<Course>();
        }

        // Initialize the system and load mock data
        public void InitializeSystem()
        {
            Console.WriteLine("Initializing system...");
            LoadMockData();
            //webScraper.ScrapeData();
            //webScraper.ScrapeCourses();

            // Log loaded courses for debugging purposes still
            var all = courseManager.GetAllCourses();
            Console.WriteLine($"Below are the loaded {all.Count} courses:");
            foreach (var c in all)
                Console.WriteLine($" - {c.CourseCode} : {c.Title}");
        }

        // Load mock data for my testing purposes
        public void LoadMockData()
        {
            Console.WriteLine("Loading mock data...");

            var course1 = new Course
            {
                CourseCode = "CS 121",
                Title = "Intro to Programming",
                Prefix = "CS",
                Credits = 4,
                Description = "Basic programming concepts.",
                UCoreCategory = "QUAN",
                ClassNumberInt = 12345,
                Status = "Open",
                SpotsLeft = 10,
                Section = "01",
                Header = "Computer Science"
            };

            var course2 = new Course
            {
                CourseCode = "HIST 105",
                Title = "World History",
                Prefix = "HIST",
                Credits = 4,
                UCoreCategory = "HUM",
                ClassNumberInt = 45678,
                Status = "Open",
                SpotsLeft = 15,
                Section = "01",
                Header = "History"
            };

            var course3 = new Course
            {
                CourseCode = "CS 122",
                Title = "Advanced Programming",
                Prefix = "CS",
                Credits = 4,
                UCoreCategory = "QUAN",
                ClassNumberInt = 67890,
                Status = "Open",
                SpotsLeft = 20,
                Section = "01",
                Header = "Computer Science"
            };

            var course4 = new Course
            {
                CourseCode = "MATH 101",
                Title = "College Algebra",
                Prefix = "MATH",
                Credits = 3,
                UCoreCategory = "QUAN",
                ClassNumberInt = 78901,
                Status = "Open",
                SpotsLeft = 25,
                Section = "01",
                Header = "Mathematics"
            };

            courseManager.AddCourse(course1);
            courseManager.AddCourse(course2);
            courseManager.AddCourse(course3);
            courseManager.AddCourse(course4);

            var major = new Major { Name = "Computer Science" };
            major.AddRequirement(new List<string> { "CS 121" });
            major.AddRequirement(new List<string> { "CS 122" });

            var minor = new Minor { Name = "History" };
            minor.AddRequirement(new List<string> { "HIST 105" });

            InitializeStudentData(new List<string> { "CS 121" }, major, minor);

            Console.WriteLine($"Successfully Loaded {courseManager.GetAllCourses().Count} courses into CourseManager");
        }
    }
}

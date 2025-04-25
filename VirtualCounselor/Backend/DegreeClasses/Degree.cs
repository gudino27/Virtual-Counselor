namespace VirtualCounselor
{
    /// <summary>
    /// Storage class designed to hold all info on any given degree.
    /// This includes information like course requirements.
    /// </summary>
    public class Degree
    {
        private string name = string.Empty;
        private int id = -1;
        private List<(Course,bool)> requiredCourses = new List<(Course, bool)>();

        public Degree(string newName, int newId)
        {
            this.name = newName;
            this.id = newId;
        }

        public string Name
        {
            get { return name; }
        }

        public int Id
        {
            get { return id; }
        }

        public List<(Course, bool)> Courses
        {
            get { return requiredCourses; }
        }

        // Method to add a test course to the degree course list
        public void AddTestCourse(string name, int numID)
        {
            this.requiredCourses.Add((new Course(name, numID), false));
        }
    }
}

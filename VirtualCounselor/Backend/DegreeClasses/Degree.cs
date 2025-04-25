namespace VirtualCounselor
{
    /// <summary>
    /// Storage class designed to hold all info (its name, id, and required course) on any given degree.
    /// This includes information like course requirements.
    /// </summary>
    public class Degree
    {
        private string name = string.Empty;
        private int id = -1;
        private readonly List<(Course course, bool taken)> requiredCourses
            = new List<(Course, bool)>();

        public Degree(string newName, int newId)
        {
            this.name = newName ?? throw new ArgumentNullException(nameof(newName));
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

        /// <summary>Add a required course (taken flag optional).</summary>
        public void AddRequiredCourse(Course course, bool isTaken = false)
        {
            if (course == null) throw new ArgumentNullException(nameof(course));
            requiredCourses.Add((course, isTaken));
        }

        /// <summary>Returns (Course, takenFlag) for this degree.</summary>
        public List<(Course course, bool taken)> GetRequiredCourses()
            => requiredCourses.ToList();
    }
}

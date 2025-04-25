namespace VirtualCounselor
{
    /// <summary>
    /// This is a storage class designed to be treated like a struct.
    /// CourseManager will contain a list of these course objects.
    /// </summary>
    public class Course
    {
        //FOR NOW only stores the name and the id chosen by the course manager.
        //will store more data in the future but this is all I need for now for UI
        private string name = string.Empty;
        private int id = -1;
        /// <summary>e.g. "CPT S 121"</summary>
        public string CourseCode { get; set; } = string.Empty;
        /// <summary>e.g. "Program Design and Development"</summary>
        public string Title { get; set; } = string.Empty;
        /// <summary>credit hours</summary>
        public int Credits { get; set; }
        /// <summary>optional UCore category code, e.g. "WRTG"</summary>
        public string? UCoreCategory { get; set; }

        public Course() { }

        public Course(string newName, int newId, string courseCode, string title, int credits, string? ucore = null)
        {
            this.name = newName;
            this.id = newId;
            this.CourseCode = courseCode;
            this.Title = title;
            this.Credits = credits;
            this.UCoreCategory = ucore;
        }

        public int Id
        {
            get { return id; }
        }

        public string Name{ 
            get { return name; } 
        }
    }
}

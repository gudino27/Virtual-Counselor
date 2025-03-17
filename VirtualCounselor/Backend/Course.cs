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

        public Course(string newName, int newId)
        {
            this.name = newName;
            this.id = newId;
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

namespace VirtualCounselor
{
    /// <summary>
    /// Child class of Degree that stores different data than a Minor.
    /// </summary>
    public class Major : Degree
    {
        public int Credits { get; set; } = 120;

        // List of required classes, stored in a string/int Tuple (class name / credits)
        public List<Tuple<string, int>> ClassList { get; set; }

        // Print the degree description paragraph to the console + the list of classes required
        public override void PrintDegreInfo()
        {
            Console.WriteLine(DegreeDescription);
            for (int i = 0; i < this.ClassList.Count; i++)
            {
                Console.WriteLine($"\nCourse: {ClassList[i].Item1} , Credits: {ClassList[i].Item2}");
            }
        }
    }
}

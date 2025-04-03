namespace VirtualCounselor
{
    /// <summary>
    /// Storage class designed to hold all info on any given degree.
    /// This includes information like course requirements.
    /// </summary>
    public class Degree
    {
        public string DegreeDescription { get; set; }

        // Print the degree description paragraph to the console
        public virtual void PrintDegreInfo()
        {
            Console.WriteLine(DegreeDescription);
        }
    }
}

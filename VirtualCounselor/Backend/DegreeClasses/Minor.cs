using System.Collections.Generic;

namespace VirtualCounselor
{
    /// <summary>
    /// Child class of Degree that stores different data than a Major.
    /// </summary>
    public class Minor : Degree
    {
        // Total credits necessary for the minor
        public int TotalCredits {  get; set; }

        // Print the degree description paragraph to the console + total required credits
        public override void PrintDegreInfo()
        {
            Console.WriteLine($"Required Credits: {TotalCredits}");
            Console.WriteLine(DegreeDescription);
        }
    }
}

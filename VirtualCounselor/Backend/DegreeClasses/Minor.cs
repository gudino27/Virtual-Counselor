namespace VirtualCounselor
{
    /// <summary>
    /// Child class of Degree that stores different data than a Major.
    /// </summary>
    public class Minor : Degree
    {
        /// <summary>
        /// Apparently, most degree programs in the US typically require 18-24 credits for a minor.With WSU, for our college, it is 20 max.
        /// </summary>
        public int Credits { get; set; } = 20;

        // Total credits necessary for the minor
        public int TotalCredits { get; set; }

        // Print the degree description paragraph to the console + total required credits
        public override void PrintDegreInfo()
        {
            Console.WriteLine($"Required Credits: {TotalCredits}");
            Console.WriteLine(DegreeDescription);
        }
    }
}

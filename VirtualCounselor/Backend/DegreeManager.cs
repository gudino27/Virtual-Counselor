namespace VirtualCounselor
{
    /// <summary>
    /// DegreeManager will handle all of the degree data parsed by the PDF reader (or webscraper I can't remember?).
    /// It will store all info on each degree read in by having a list of Degrees.
    /// </summary>
    public static class DegreeManager
    {
        private static readonly Dictionary<int, Degree> degreeList
           = new Dictionary<int, Degree>();
        private static int currentID = 1;

        /// <summary>Adds a new degree (name) and returns it.</summary>
        public static Degree AddDegree(string degreeName)
        {
            var deg = new Degree(degreeName, currentID++);
            degreeList[deg.Id] = deg;
            return deg;
        }

        /// <summary>
        /// Takes an id and returns the degree associated.
        /// </summary>
        /// <param name="id">the id of the degree.</param>
        /// <returns>the degree object.</returns>
        public static Degree GetDegreeByID(int id)
        {
            if (degreeList.ContainsKey(id))
            {
                return degreeList[id];
            }
            else
            {
                return null;
            }
        }

        /// <summary>All degrees loaded so far.</summary>
        public static List<Degree> GetDegreeList()
            => new List<Degree>(degreeList.Values);
    }
}

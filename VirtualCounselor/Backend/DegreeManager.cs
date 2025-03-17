namespace VirtualCounselor
{
    /// <summary>
    /// DegreeManager will handle all of the data parsed by the PDF reader (or webscraper I can't remember?).
    /// It will store all info on each degree read in by having a list of Degrees.
    /// </summary>
    public static class DegreeManager
    {
        private static int currentID = 0;
        private static Dictionary<int, Degree> degreeList = new Dictionary<int, Degree>();

        /// <summary>
        /// Method called to create a degree.
        /// </summary>
        /// <param name="degreeName">the name of the degree</param>
        public static void AddDegree(string degreeName)
        {
            degreeList.Add(currentID, new Degree(degreeName, currentID));
            currentID++;
        }

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

        public static List<Degree> GetDegreeList()
        {
            List<Degree> degrees = new List<Degree>();
            foreach (var degree in degreeList.Values)
            {
                degrees.Add(degree);
            }
            return degrees;
        }
    }
}

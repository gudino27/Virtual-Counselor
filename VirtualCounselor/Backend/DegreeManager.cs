using System.Collections.Generic;
using System.Linq;

namespace VirtualCounselor
{
    /// <summary>
    /// DegreeManager will handle all of the data parsed by the PDF reader (or webscraper I can't remember?).
    /// It will store all info on each degree read in by having a list of Degrees, each of which can be a Major or Minor.
    /// </summary>
    public class DegreeManager
    {
        //private List<Degree> degreeList;

        private List<Degree> allDegrees = new();

        public Major CurrentMajor { get; set; }
        public Major CurrentDoubleMajor { get; set; }
        public Minor CurrentMinor { get; set; }

        public void AddDegree(Degree degree)
        {
            if (degree != null && !allDegrees.Any(d => d.Name == degree.Name))
                allDegrees.Add(degree);
        }

        public Degree GetDegreeByName(string name)
        {
            return allDegrees.FirstOrDefault(d => d.Name == name);
        }

        public List<Degree> GetActiveDegrees()
        {
            var result = new List<Degree>();
            if (CurrentMajor != null) result.Add(CurrentMajor);
            if (CurrentMinor != null) result.Add(CurrentMinor);
            if (CurrentDoubleMajor != null) result.Add(CurrentDoubleMajor);
            return result;
        }

        public List<Degree> GetAllDegrees() => allDegrees;
    }
}
